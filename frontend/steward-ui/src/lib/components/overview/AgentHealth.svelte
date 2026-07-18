<script>
    import Card from "../Card.svelte";

    let lastChecked = "10 seconds ago";

    function refresh() {
        // TODO: call health endpoint
        lastChecked = "just now";
    }
</script>

<Card title="Agent Health">
    {#snippet actions()}
        <button class="cta-button" onclick={refresh}> ↻ Refresh </button>
    {/snippet}

    <div class="health">
        <div class="status">
            <span class="dot"></span>
            <span>Healthy</span>
        </div>

        <div class="details">
            <div>
                <strong>5</strong>
                <span>Agents Online</span>
            </div>

            <div>
                <strong>{lastChecked}</strong>
                <span>Last Check</span>
            </div>
        </div>
    </div>
</Card>

<style>
    .status {
        display: flex;
        align-items: center;
        gap: var(--space-2);

        font-weight: 600;
        color: var(--color-success);
    }

    .dot {
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: var(--color-success);
    }

    .details {
        display: flex;
        gap: var(--space-7);
        margin-top: var(--space-6);
    }

    .details div {
        display: flex;
        flex-direction: column;
    }

    strong {
        font-size: 1.4rem;
    }

    span {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }
</style>
