<script>
    let { title = "", children, actions = null } = $props();
</script>

<div class="card">
    {#if title || actions}
        <div class="card-header">
            {#if title}
                <h3>{title}</h3>
            {/if}

            {#if actions}
                {@render actions()}
            {/if}
        </div>
    {/if}

    {@render children()}
</div>

<style>
    .card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: var(--space-4);
    }

    .card {
        background: var(--color-surface);
        border: 1px solid rgba(134, 188, 73, 0.12);
        border-radius: var(--radius-md);
        padding: var(--space-5);

        box-shadow:
            0 1px 2px rgba(0, 0, 0, 0.25),
            0 6px 24px rgba(0, 0, 0, 0.15);

        transition:
            border-color 0.2s,
            box-shadow 0.2s;

        grid-column: span var(--col-span, 1);
        grid-row: span var(--row-span, 1);
    }

    .card:hover {
        border-color: rgba(255, 255, 255, 0.131);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.28);
    }
</style>
