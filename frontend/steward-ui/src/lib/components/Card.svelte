<div class="card">
    <slot />
</div>

<style>
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
    }

    .card:hover {
        border-color: rgba(255, 255, 255, 0.131);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.28);
    }
</style>
