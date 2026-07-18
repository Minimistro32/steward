<script>
    import Card from "../Card.svelte";

    let devices = [
        "Engineering Laptop Pool",
        "Production Server",
        "AWS Infrastructure",
        "AWS Infrastructure",
        "AWS Infrastructure",
        "AWS Infrastructure",
        "AWS Infrastructure",
        "AWS Infrastructure",
    ];

    let users = ["Sarah", "Marcus", "Jamie"];

    function refresh() {
        // TODO: call health endpoint
        lastRefreshed = "just now";
    }

    let lastRefreshed = "3 minutes ago";
    let disabled = "0";
    let online = "5";
    let agents = [1, 2, 3, 4, 5, 6];
</script>

<Card title="Summary">
    {#snippet actions()}
        <button class="cta-button" onclick={refresh}> ↻ Refresh </button>
    {/snippet}

    <div class="summary">
        <div class="metric">
            <strong>{online} / {agents.length}</strong>
            <span>Agents Online</span>
        </div>

        <div class="metric">
            <strong class="disabled">{disabled}</strong>
            <span>Agents Disabled</span>
        </div>

        <div class="management-group">
            <h4>Managed Devices</h4>

            <ul>
                {#each devices as device}
                    <li>{device}</li>
                {/each}
            </ul>
        </div>

        <div class="management-group">
            <h4>Managed Users</h4>

            <ul>
                {#each users as user}
                    <li>{user}</li>
                {/each}
            </ul>
        </div>
    </div>

    <div class="refresh">
        Last refreshed {lastRefreshed}
    </div>
</Card>

<style>
    .metric {
        padding-bottom: var(--space-4);
    }

    .metric strong {
        display: block;
        font-size: 1.75rem;
        font-weight: 600;
    }

    .metric span {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .disabled {
        color: var(--color-warning);
    }

    .summary {
        display: grid;
        grid-template-columns: 1fr 1fr 1.5fr 1.5fr;
        gap: var(--space-7);

        margin-left: var(--space-6);

        align-items: center;
    }

    .management-group h4 {
        margin: 0 0 var(--space-2);

        font-size: 0.8rem;
        text-transform: uppercase;
        letter-spacing: 0.05em;

        color: var(--color-text-muted);
    }

    ul {
        margin: 0;
        padding-left: var(--space-5);

        position: relative;
        max-height: 9vh;
        overflow-y: auto;
    }

    li {
        margin-bottom: var(--space-1);
    }

    .refresh {
        margin-top: var(--space-2);
        text-align: right;
        color: var(--color-text-muted);
        font-size: 0.8rem;
    }
</style>
