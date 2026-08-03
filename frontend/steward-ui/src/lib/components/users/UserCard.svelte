<script lang="ts">
    import Card from "../ui/Card.svelte";

    import type { User } from "../../models/wards";
    import type { Agent } from "../../models/agents";

    type Props = {
        user: User;
        agents: Agent[];

        onAssign: (user: User, agentId: string, deviceId: string) => void;

        onRemove: (user: User, agentId: string, deviceId: string) => void;
    };

    const { user, agents, onAssign, onRemove }: Props = $props();

    const assignedDevices = $derived.by(() => {
        return Object.entries(user.agentSelections).flatMap(
            ([agentId, selection]) => {
                const agent = agents.find((a) => a.agentId === agentId);

                if (!agent) {
                    return [];
                }

                return selection.deviceIds.flatMap((deviceId) => {
                    const device = agent.devices.find((d) => d.id === deviceId);

                    return device
                        ? [
                              {
                                  agentId,
                                  agent,
                                  device,
                              },
                          ]
                        : [];
                });
            },
        );
    });

    function drop(event: DragEvent) {
        event.preventDefault();

        const agentId = event.dataTransfer?.getData("agentId");

        const deviceId = event.dataTransfer?.getData("deviceId");

        if (agentId && deviceId) {
            onAssign(user, agentId, deviceId);
        }
    }

    function dragOver(event: DragEvent) {
        event.preventDefault();

        if (event.dataTransfer) {
            event.dataTransfer.dropEffect = "copy";
        }
    }
</script>

<Card>
    <div
        class="card"
        role="region"
        aria-label={`Devices assigned to ${user.name}`}
        ondragover={dragOver}
        ondrop={drop}
    >
        <div class="header">
            <h2>
                {user.name}
            </h2>

            <span>
                {assignedDevices.length}
                device{assignedDevices.length === 1 ? "" : "s"}
            </span>
        </div>

        <div class="devices">
            {#if assignedDevices.length === 0}
                <p class="empty">Drop devices here</p>
            {:else}
                {#each assignedDevices as item}
                    <div class="device-chip">
                        <span>
                            {item.device.name}
                        </span>

                        <button
                            onclick={() =>
                                onRemove(user, item.agentId, item.device.id)}
                            aria-label="Remove device"
                        >
                            &#215;
                        </button>
                    </div>
                {/each}
            {/if}
        </div>
    </div>
</Card>

<style>
    .card {
        min-height: 120px;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;

        margin-bottom: var(--space-4);
    }

    h2 {
        margin: 0;
    }

    .header span {
        color: var(--color-text-muted);
        font-size: 0.8rem;
    }

    .devices {
        display: flex;
        flex-wrap: wrap;

        gap: var(--space-2);

        min-height: 40px;

        padding: var(--space-3);

        border: 1px dashed var(--color-border);
        border-radius: var(--radius-md);
    }

    .device-chip {
        display: flex;
        align-items: center;

        gap: var(--space-2);

        background: var(--color-surface-raised);

        padding: var(--space-2) var(--space-3);

        border-radius: var(--radius-sm);

        font-size: 0.85rem;
    }

    button {
        border: none;
        background: transparent;

        color: var(--color-text-muted);

        cursor: pointer;

        font-size: 1rem;
        line-height: 1;
    }

    button:hover {
        color: var(--color-failure);
    }

    .empty {
        margin: 0;

        width: 100%;

        text-align: center;

        color: var(--color-text-muted);

        font-size: 0.85rem;
    }
</style>
