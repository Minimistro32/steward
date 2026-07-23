<script lang="ts">
    import { slide } from "svelte/transition";
    import Card from "../ui/Card.svelte";

    import type { Agent } from "../../models/agents/Agent";
    import { AgentStatus } from "../../models/agents/Agent";

    const { agent } = $props<{ agent: Agent }>();

    let expanded = $state(false);

    // DRY up with the date formatter in AgentSummary?
    function formatLastSeen(date?: Date) {
        if (!date) return "Online";

        const minutes = Math.floor((Date.now() - date.getTime()) / 60000);

        if (minutes < 1) return "Just now";
        if (minutes === 1) return "1 minute ago";
        if (minutes < 60) return `${minutes} minutes ago`;

        const hours = Math.floor(minutes / 60);

        if (hours === 1) return "1 hour ago";

        return `${hours} hours ago`;
    }
</script>

<Card>
    {#snippet actions()}
        <button
            class="menu"
            onclick={(event) => {
                event.stopPropagation();
                // TODO: Open agent actions menu
            }}
            aria-label="Agent actions"
        >
            ⋮
        </button>
    {/snippet}

    <div
        class="content"
        role="button"
        tabindex="0"
        onclick={() => (expanded = !expanded)}
        onkeydown={(event) => {
            if (event.key === "Enter" || event.key === " ") {
                expanded = !expanded;
            }
        }}
    >
        <div class="header">
            <div class="identity">
                <div
                    class="dot"
                    class:online={agent.status === AgentStatus.Online}
                    class:offline={agent.status === AgentStatus.Offline}
                ></div>

                <div>
                    <h3>{agent.name}</h3>
                    {#if agent.lastSeen}
                        <p>{formatLastSeen(agent.lastSeen)}</p>
                    {:else}
                        <p>Online</p>
                    {/if}
                </div>
            </div>
        </div>

        {#if expanded}
            <div class="details" transition:slide={{ duration: 200 }}>
                <section>
                    <h4>Resources</h4>

                    <ul>
                        {#each agent.resources as resource}
                            <li>{resource.name}</li>
                        {/each}
                    </ul>
                </section>

                <section>
                    <h4>Users</h4>

                    <ul>
                        {#each agent.userList as user}
                            <li>{user}</li>
                        {/each}
                    </ul>
                </section>

                <section>
                    <h4>Devices</h4>

                    <ul>
                        {#each agent.deviceList as device}
                            <li>{device}</li>
                        {/each}
                    </ul>
                </section>
            </div>
        {:else}
            <div class="stats">
                <div>
                    <strong>{agent.resources.length}</strong>
                    <span>Resources</span>
                </div>

                <div>
                    <strong>2</strong>
                    <span>Users</span>
                </div>

                <div>
                    <strong>1</strong>
                    <span>Devices</span>
                </div>
            </div>
        {/if}
    </div>
</Card>

<style>
    .content {
        width: 100%;
        cursor: pointer;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;

        margin-bottom: var(--space-6);
    }

    .identity {
        display: flex;
        align-items: center;
        gap: var(--space-3);
    }

    h3 {
        margin: 0;
    }

    p {
        margin: 0;

        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .dot {
        width: 12px;
        height: 12px;

        border-radius: 50%;
        flex-shrink: 0;
    }

    .dot.online {
        background: var(--color-success);
    }

    .dot.offline {
        background: var(--color-failure);
    }

    .stats {
        display: flex;
        gap: var(--space-7);
    }

    .stats strong {
        display: block;

        font-size: 1.5rem;
        font-weight: 600;
    }

    span {
        color: var(--color-text-muted);
        font-size: 0.8rem;
    }

    .details {
        display: flex;
        flex-direction: column;

        gap: var(--space-6);
    }

    .details section {
        border-top: 1px solid var(--color-border);
        padding-top: var(--space-3);
    }

    .details h4 {
        margin: 0 0 var(--space-2);

        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.05em;

        color: var(--color-text-muted);
    }

    ul {
        margin: 0;
        padding-left: var(--space-4);
    }

    li {
        margin-bottom: var(--space-1);

        font-size: 0.9rem;
    }

    .menu {
        width: 32px;
        height: 32px;

        background: transparent;
        border: none;
        border-radius: var(--radius-sm);

        color: var(--color-text-muted);

        cursor: pointer;
    }

    .menu:hover {
        background: var(--color-surface-raised);
        color: var(--color-text);
    }
</style>
