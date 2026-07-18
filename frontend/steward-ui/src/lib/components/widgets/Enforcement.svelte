<script>
    import Card from "../Card.svelte";
</script>

<Card title="Enforcement">

    <div class="enforcement">

        <div class="status text-muted">
            <span class="dot"></span>
            Enabled
        </div>

        <div class="stats">
            <div>
                <strong>18</strong>
                <span>Active Policies</span>
            </div>

            <div>
                <strong>1,284</strong>
                <span>Evaluations / hr</span>
            </div>
        </div>

    </div>

</Card>

<style>
    .status {
        font-weight: 600;
        font-size: 0.85rem;

        display: flex;
        align-items: center;
        gap: var(--space-2);
    }

    .dot {
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: var(--color-success);
    }

    .stats {
        display: flex;
        gap: var(--space-6);
        margin-top: var(--space-5);
    }

    strong {
        font-size: 1.5rem;
        display: block;
    }

    span {
        color: var(--color-text-muted);
        font-size: .85rem;
    }
</style>