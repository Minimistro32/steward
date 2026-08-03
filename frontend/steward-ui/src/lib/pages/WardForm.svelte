<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import Card from "../components/ui/Card.svelte";
    import Checkbox from "../components/ui/Checkbox.svelte";

    import type { Ward, User } from "../models/wards";
    import { AgentStatus, type Agent } from "../models/agents";

    import { createWard, getWard, updateWard, getUsers } from "../api/wardApi";
    import { getAgents } from "../api/agentApi";

    let { params } = $props();

    let ward = $state<Ward>({
        id: "",
        name: "",
        tags: [],
        userIds: [],
        agentSelections: {},
    });

    let users = $state<User[]>([]);
    let agents = $state<Agent[]>([]);

    let loading = $state(true);
    let notFound = $state(false);

    let errors = $state<string[]>([]);

    const isNew = $derived(!params?.id);

    onMount(async () => {
        [users, agents] = await Promise.all([getUsers(), getAgents()]);

        if (params?.id) {
            const found = await getWard(params.id);

            if (!found) {
                notFound = true;
                loading = false;
                return;
            }

            ward = found;
        }

        loading = false;
    });

    const availableDevicesByAgent = $derived.by(() => {
        return agents
            .filter((agent) => agent.status !== AgentStatus.Disabled)
            .map((agent) => {
                const devices = agent.devices.filter((device) =>
                    users.some(
                        (user) =>
                            ward.userIds.includes(user.id) &&
                            user.agentSelections[
                                agent.agentId
                            ]?.deviceIds.includes(device.id),
                    ),
                );

                return {
                    agent,
                    devices,
                };
            })
            .filter((group) => group.devices.length > 0);
    });

    const availableResourcesByAgent = $derived.by(() => {
        return agents
            .filter((agent) => agent.status !== AgentStatus.Disabled)
            .map((agent) => {
                const selectedDevices =
                    ward.agentSelections[agent.agentId]?.deviceIds ?? [];

                const hasDevices = selectedDevices.length > 0;

                return {
                    agent,
                    resources: hasDevices ? agent.resources : [],
                };
            })
            .filter((group) => group.resources.length > 0);
    });

    function getAgentSelection(agentId: string) {
        return (ward.agentSelections[agentId] ??= {
            deviceIds: [],
            resourceIds: [],
        });
    }

    function toggle(list: string[], id: string) {
        return list.includes(id) ? list.filter((x) => x !== id) : [...list, id];
    }

    function toggleDevice(agentId: string, deviceId: string) {
        const selection = getAgentSelection(agentId);

        selection.deviceIds = toggle(selection.deviceIds, deviceId);
    }

    function toggleResource(agentId: string, resourceId: string) {
        const selection = getAgentSelection(agentId);

        selection.resourceIds = toggle(selection.resourceIds, resourceId);
    }

    function pruneSelections() {
        const availableDevices = new Map(
            availableDevicesByAgent.map((group) => [
                group.agent.agentId,
                new Set(group.devices.map((d) => d.id)),
            ]),
        );

        const availableResources = new Map(
            availableResourcesByAgent.map((group) => [
                group.agent.agentId,
                new Set(group.resources.map((r) => r.id)),
            ]),
        );

        for (const agentId of Object.keys(ward.agentSelections)) {
            const selection = ward.agentSelections[agentId];

            const validDevices = availableDevices.get(agentId) ?? new Set();

            selection.deviceIds = selection.deviceIds.filter((id) =>
                validDevices.has(id),
            );

            const validResources = availableResources.get(agentId) ?? new Set();

            selection.resourceIds = selection.resourceIds.filter((id) =>
                validResources.has(id),
            );

            if (
                selection.deviceIds.length === 0 &&
                selection.resourceIds.length === 0
            ) {
                delete ward.agentSelections[agentId];
            }
        }
    }

    function validateWard() {
        if (!ward) return false;

        errors = [];

        if (!ward.name.trim()) {
            errors.push("Ward name is required.");
        }

        if (ward.userIds.length === 0) {
            errors.push("Select at least one user.");
        }

        const hasDevices = Object.values(ward.agentSelections).some(
            (selection) => selection.deviceIds.length > 0,
        );

        if (!hasDevices) {
            errors.push("Select at least one device.");
        }

        return errors.length === 0;
    }

    async function saveWard() {
        if (!ward || !validateWard()) {
            return;
        }

        if (isNew) {
            await createWard(ward);
        } else {
            await updateWard(ward);
        }

        // TODO navigate back
    }
</script>

{#if loading}
    <p>Loading...</p>
{:else if notFound}
    <PageHeader title="Ward Not Found">
        {#snippet subtitle()}
            The requested ward could not be found.
        {/snippet}
    </PageHeader>
{:else if ward}
    <div class="centered">
        <PageHeader
            title={isNew ? "Create Ward" : "Edit Ward"}
            --margin-bottom="var(--space-4)"
        >
            {#snippet subtitle()}
                Group users, devices, and resources into a reusable policy
                target.
            {/snippet}
        </PageHeader>

        <div class="editor">
            <Card>
                <h2>General</h2>

                <label>
                    Ward Name
                    <input bind:value={ward.name} />
                </label>

                <label>
                    Tags
                    <input
                        bind:value={ward.tags}
                        placeholder="gaming, family"
                    />
                </label>
            </Card>

            <Card>
                <h2>Users</h2>

                <p class="text-muted">Select the users managed by this ward.</p>

                {#each users as user}
                    <Checkbox
                        label={user.name}
                        checked={ward.userIds.includes(user.id)}
                        onchange={() => {
                            ward.userIds = toggle(ward.userIds, user.id);
                            pruneSelections();
                        }}
                    />
                {/each}
            </Card>

            {#if availableDevicesByAgent.length}
                <Card>
                    <h2>Devices</h2>

                    <p class="text-muted">
                        Devices accessible to selected users.
                    </p>

                    {#each availableDevicesByAgent as group}
                        <div class="resource-group">
                            <h3>{group.agent.name}</h3>

                            {#each group.devices as device}
                                <Checkbox
                                    label={device.name}
                                    checked={ward.agentSelections[
                                        group.agent.agentId
                                    ]?.deviceIds.includes(device.id) ?? false}
                                    onchange={() => {
                                        toggleDevice(
                                            group.agent.agentId,
                                            device.id,
                                        );
                                        pruneSelections();
                                    }}
                                />
                            {/each}
                        </div>
                    {/each}
                </Card>
            {/if}

            {#if availableResourcesByAgent.length}
                <Card>
                    <h2>Resources</h2>

                    <p class="text-muted">
                        Resources agents can control on selected devices.
                    </p>

                    {#each availableResourcesByAgent as group}
                        <div class="resource-group">
                            <h3>{group.agent.name}</h3>

                            {#each group.resources as resource}
                                <Checkbox
                                    label={resource.name}
                                    checked={ward.agentSelections[
                                        group.agent.agentId
                                    ]?.resourceIds.includes(resource.id) ??
                                        false}
                                    onchange={() =>
                                        toggleResource(
                                            group.agent.agentId,
                                            resource.id,
                                        )}
                                />
                            {/each}
                        </div>
                    {/each}
                </Card>
            {/if}

            {#if errors.length}
                <div class="errors">
                    {#each errors as error}
                        <p>{error}</p>
                    {/each}
                </div>
            {/if}

            <div class="actions">
                <a href="#/wards">
                    <button class="cta-button"> Cancel </button>
                </a>

                <button class="primary" onclick={saveWard}> Save Ward </button>
            </div>
        </div>
    </div>
{/if}

<style>
    h3 {
        color: var(--color-text-muted);

        margin: var(--space-4) 0 var(--space-1);
        color: var(--color-text-muted);
        font-size: 0.9rem;
    }

    .centered {
        margin: 0 auto;
        max-width: 50vw;
    }

    .editor {
        display: flex;
        flex-direction: column;
        gap: var(--space-4);
    }

    label {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
        margin-bottom: var(--space-4);
        color: var(--color-text-muted);
        font-size: 0.9rem;
    }

    .actions {
        display: flex;
        justify-content: flex-end;
        gap: var(--space-3);
        grid-column: span 2;
    }

    .primary {
        background: var(--color-brand);
        color: white;
        border: none;
        padding: var(--space-2) var(--space-4);
        border-radius: var(--radius-sm);
        cursor: pointer;
    }

    a {
        color: var(--color-text-muted);
        text-decoration: none;
        font-weight: bold;
    }

    a:hover {
        color: var(--color-brand-light);
    }

    .errors {
        background: rgba(229, 83, 83, 0.1);
        border: 1px solid var(--color-failure);
        border-radius: var(--radius-md);

        padding: var(--space-4);

        margin-bottom: var(--space-4);
    }

    .errors p {
        color: var(--color-failure);
        margin: 0;
    }
</style>
