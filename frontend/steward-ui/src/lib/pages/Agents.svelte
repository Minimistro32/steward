<script>
    import Card from "../components/Card.svelte";
    import PageHeader from "../components/PageHeader.svelte";

    let agents = [
        {
            name: "engineering-agent-01",
            status: "online",
            ward: "Engineering",
            lastSeen: "2 minutes ago",
            resources: 24,
            devices: 12,
        },
        {
            name: "production-agent-01",
            status: "online",
            ward: "Production",
            lastSeen: "8 minutes ago",
            resources: 14,
            devices: 6,
        },
        {
            name: "research-agent-01",
            status: "offline",
            ward: "Research",
            lastSeen: "3 hours ago",
            resources: 8,
            devices: 4,
        },
    ];

    let lastRefresh = "3 minutes ago";
</script>

<PageHeader title="Agents">
    {#snippet subtitle()}
        Agents watch over wards, discover resources, and enforce policies.
    {/snippet}
</PageHeader>

<div class="agent-grid">
    <Card title="Agent Health">
        {#snippet actions()}
            <button class="refresh"> Refresh </button>
        {/snippet}

        <div class="health">
            <div>
                <strong>
                    {agents.filter((a) => a.status === "online").length}
                </strong>
                <span>Online</span>
            </div>

            <div>
                <strong class="offline">
                    {agents.filter((a) => a.status === "offline").length}
                </strong>
                <span>Offline</span>
            </div>
        </div>

        <p class="updated">
            Last refresh {lastRefresh}
        </p>
    </Card>

    <Card title="Registered Agents">
        <div class="agents">
            {#each agents as agent}
                <div class="agent">
                    <div class={`dot ${agent.status}`}></div>

                    <div class="details">
                        <div class="name">
                            {agent.name}
                        </div>

                        <div class="meta">
                            {agent.ward} · Last seen {agent.lastSeen}
                        </div>

                        <div class="stats">
                            {agent.resources} resources ·
                            {agent.devices} devices
                        </div>
                    </div>

                    <button class="menu"> ⋮ </button>
                </div>
            {/each}
        </div>
    </Card>
</div>

<style>
    .agent-grid {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: var(--space-5);
    }

    .health {
        display: flex;
        gap: var(--space-8);
    }

    strong {
        display: block;
        font-size: 2rem;
    }

    .offline {
        color: var(--color-failure);
    }

    span,
    .meta,
    .stats,
    .updated {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .updated {
        margin-top: var(--space-4);
    }

    .agents {
        display: flex;
        flex-direction: column;
        gap: var(--space-3);
    }

    .agent {
        display: flex;
        align-items: center;
        gap: var(--space-4);

        padding: var(--space-3);

        border-radius: var(--radius-sm);

        transition: background 0.2s;
    }

    .agent:hover {
        background: var(--color-surface-raised);
    }

    .dot {
        width: 10px;
        height: 10px;

        border-radius: 50%;
    }

    .dot.online {
        background: var(--color-success);
    }

    .dot.offline {
        background: var(--color-failure);
    }

    .details {
        flex: 1;
    }

    .name {
        font-weight: 600;
    }

    .menu,
    .refresh {
        background: transparent;
        border: 1px solid var(--color-border);

        border-radius: var(--radius-sm);

        padding: var(--space-2) var(--space-3);

        color: var(--color-text-muted);

        cursor: pointer;
    }

    .menu:hover,
    .refresh:hover {
        color: var(--color-text);
        border-color: rgba(255, 255, 255, 0.3);
    }
</style>
