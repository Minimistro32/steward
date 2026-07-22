<script>
    import Card from "../ui/Card.svelte";
    import StatusDot from "../ui/StatusDot.svelte";

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
        <StatusDot label="Healthy" --font-size=0.9rem />

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
