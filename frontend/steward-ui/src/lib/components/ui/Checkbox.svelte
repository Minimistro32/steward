<script lang="ts">
    export let checked = false;
    export let label = "";
</script>

<label class="checkbox">
    <input type="checkbox" bind:checked />

    <span class="box"></span>

    <span class="label">
        {label}
    </span>
</label>

<style>
    .checkbox {
        display: flex;

        align-items: center;

        gap: var(--space-3);

        cursor: pointer;

        color: var(--color-text);
    }

    .checkbox input {
        position: absolute;

        opacity: 0;

        pointer-events: none;
    }

    .box {
        width: 18px;
        height: 18px;

        border: 1px solid var(--color-border);

        border-radius: var(--radius-sm);

        background: var(--color-surface-raised);

        display: flex;

        align-items: center;

        justify-content: center;

        transition:
            background 0.15s,
            border-color 0.15s;
    }

    .checkbox:hover .box {
        border-color: var(--color-brand-light);
    }

    .checkbox input:checked + .box {
        background: var(--color-brand);

        border-color: var(--color-brand);
    }

    .checkbox input:checked + .box::after {
        content: "";

        width: 5px;
        height: 10px;

        border: solid white;

        border-width: 0 1.5px 1.5px 0;

        transform: rotate(45deg) translateY(-1px);
    }

    .label {
        user-select: none;
    }
</style>
