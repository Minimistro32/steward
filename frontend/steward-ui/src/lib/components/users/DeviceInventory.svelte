<script lang="ts">
    import Card from "../ui/Card.svelte";
    import type { Agent } from "../../models/agents";

    import { getAgents } from "../../api/agentApi";

    type Props = {
        agents: Agent[];
    };

    const { agents }: Props = $props();

    const availableAgents = $derived(
        agents.filter((agent) => agent.devices.length > 0),
    );

    function dragStart(event: DragEvent, agent: Agent, deviceId: string) {
        event.dataTransfer?.setData("agentId", agent.agentId);

        event.dataTransfer?.setData("deviceId", deviceId);

        if (event.dataTransfer) {
            event.dataTransfer.effectAllowed = "copy";
        }
    }
</script>

<Card title="Available Devices">
    <div class="inventory">
        {#each availableAgents as agent}
            <section class="agent-group">
                <h4>
                    {agent.name}
                </h4>

                <div class="devices" role="list">
                    {#each agent.devices as device}
                        <button
                            class="device"
                            type="button"
                            draggable="true"
                            aria-label={`Drag ${device.name} to assign`}
                            ondragstart={(event) =>
                                dragStart(event, agent, device.id)}
                        >
                            {device.name}
                        </button>
                    {/each}
                </div>
            </section>
        {:else}
            <p class="empty">No devices discovered.</p>
        {/each}
    </div>
</Card>

<style>
    .inventory {
        display: flex;
        flex-direction: column;

        gap: var(--space-5);
    }

    .agent-group {
        display: flex;
        flex-direction: column;

        gap: var(--space-2);
    }

    h4 {
        margin: 0;

        padding-bottom: var(--space-1);

        border-bottom: 1px solid var(--color-border);

        color: var(--color-text-muted);

        font-size: 0.75rem;

        text-transform: uppercase;

        letter-spacing: 0.05em;
    }

    .devices {
        display: flex;
        flex-direction: column;

        gap: var(--space-2);
    }

    .device {
        width: 100%;

        color: white;

        padding: var(--space-3);

        background: var(--color-surface-raised);

        border: 1px solid var(--color-border);

        border-radius: var(--radius-md);

        font: inherit;
        text-align: left;

        cursor: grab;

        transition:
            border-color 0.15s ease,
            transform 0.15s ease;
    }

    .device:hover {
        border-color: var(--color-brand);

        transform: translateY(-1px);
    }

    .device:active {
        cursor: grabbing;
    }

    .empty {
        margin: 0;

        color: var(--color-text-muted);

        font-style: italic;
    }
</style>
