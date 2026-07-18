<script lang="ts">
    export let status: "online" | "offline" | "warning";
</script>

<div class={`badge ${status}`}>
    <span></span>
    {status}
</div>

<style>
    .badge {
        display: inline-flex;
        align-items: center;

        gap: var(--space-2);

        padding: var(--space-1) var(--space-3);

        border-radius: 999px;

        font-size: 0.85rem;
    }

    span {
        width: 0.5rem;
        height: 0.5rem;

        border-radius: 50%;
    }

    .online span {
        background: var(--color-success);
    }

    .offline span {
        background: var(--color-danger);
    }

    .warning span {
        background: var(--color-warning);
    }
</style>
