<script lang="ts">
    export let label: string;
    export let color = "var(--color-success)";
</script>

<div class="status">
    <div class="dot" style={`--dot-color: ${color}`}></div>
    <div>{label}</div>
</div>

<style>
    .status {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        color: var(--color-text-muted);
        font-size: var(--font-size, 1rem);
    }

    .dot {
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: var(--dot-color);
    }
</style>