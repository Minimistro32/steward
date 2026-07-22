<script lang="ts">
    let { title = "", children, actions = null } = $props();
</script>

<div class="card">
    {#if title}
        <div class="card-header">
            <h3>{title}</h3>

            {#if actions}
                {@render actions()}
            {/if}
        </div>
    {:else if actions}
        <div class="floating-actions">
            {@render actions()}
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
        border: 1px solid var(--color-border);
        border-radius: var(--radius-md);
        padding: var(--space-6);

        box-shadow:
            0 1px 2px rgba(0, 0, 0, 0.25),
            0 6px 24px rgba(0, 0, 0, 0.15);

        transition:
            border-color 0.2s,
            box-shadow 0.2s;

        grid-column: span var(--col-span, 1);
        grid-row: span var(--row-span, 1);

        position: relative;
    }

    .floating-actions {
        position: absolute;
        top: var(--space-3);
        right: var(--space-3);
    }

    .card:hover {
        border-color: color-mix(
            in srgb,
            var(--color-brand-light),
            transparent 80%
        );
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.28);
    }
</style>
