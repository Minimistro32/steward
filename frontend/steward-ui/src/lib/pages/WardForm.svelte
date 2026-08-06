<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import Card from "../components/ui/Card.svelte";
    import Checkbox from "../components/ui/Checkbox.svelte";

    import { type Ward, type User, type Agent, AgentStatus } from "../models";

    import {
        createWard,
        getWard,
        updateWard,
        getUsers,
        getAgents,
    } from "../api";

    let { params } = $props();

    let ward = $state<Ward>({
        name: "",
        tags: [],
        userIds: [],
        deviceIds: [],
        resourceIds: [],
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

    const availableDevicesByAgent = $derived.by(() =>
        agents
            .filter((agent) => agent.status !== AgentStatus.Disabled)
            .map((agent) => ({
                agent,
                devices: agent.devices.filter((device) =>
                    users.some(
                        (user) =>
                            ward.userIds.includes(user.id) &&
                            user.deviceIds.includes(device.id),
                    ),
                ),
            }))
            .filter((group) => group.devices.length > 0),
    );

    const availableResourcesByAgent = $derived.by(() =>
        agents
            .filter((agent) => agent.status !== AgentStatus.Disabled)
            .map((agent) => {
                const hasDevice = agent.devices.some((device) =>
                    ward.deviceIds.includes(device.id),
                );

                return {
                    agent,
                    resources: hasDevice ? agent.resources : [],
                };
            })
            .filter((group) => group.resources.length > 0),
    );

    function toggle(list: number[], id: number) {
        return list.includes(id) ? list.filter((x) => x !== id) : [...list, id];
    }

    function toggleDevice(deviceId: number) {
        ward.deviceIds = toggle(ward.deviceIds, deviceId);
    }

    function toggleResource(resourceId: number) {
        ward.resourceIds = toggle(ward.resourceIds, resourceId);
    }

    function pruneSelections() {
        const validDevices = new Set(
            availableDevicesByAgent.flatMap((x) => x.devices.map((d) => d.id)),
        );

        ward.deviceIds = ward.deviceIds.filter((id) => validDevices.has(id));

        const validResources = new Set(
            availableResourcesByAgent.flatMap((x) =>
                x.resources.map((r) => r.id),
            ),
        );

        ward.resourceIds = ward.resourceIds.filter((id) =>
            validResources.has(id),
        );
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

        const hasDevices = ward.deviceIds.length > 0;

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

        window.location.hash = "#/wards";
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
                Create a group of users, devices, and resources that will share policies.
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
                        value={ward.tags.join(", ")}
                        onchange={(event) => {
                            ward.tags = (
                                event.currentTarget as HTMLInputElement
                            ).value
                                .split(",")
                                .map((tag) => tag.trim())
                                .filter(Boolean);
                        }}
                        placeholder="gaming, family"
                    />
                </label>
            </Card>

            <Card>
                <h2>Users</h2>

                <p class="text-muted">Select users who will have independent access to this ward.</p>

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
                                    checked={ward.deviceIds.includes(device.id)}
                                    onchange={() => {
                                        toggleDevice(device.id);
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
                                    checked={ward.resourceIds.includes(
                                        resource.id,
                                    )}
                                    onchange={() => toggleResource(resource.id)}
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
        border: 1px solid var(--color-danger);
        border-radius: var(--radius-md);

        padding: var(--space-4);

        margin-bottom: var(--space-4);
    }

    .errors p {
        color: var(--color-danger);
        margin: 0;
    }
</style>
