<script lang="ts">
    export let dailyTime: string;
    export let sessionLength: string;
    export let unlocks: string;

    export let extension = false;
</script>

<div class:extension class="allowance">
    <div class="item">
        <span class="label"> Daily </span>

        <span class="value">
            {dailyTime}
        </span>
    </div>

    <div class="item">
        <span class="label"> Session </span>

        <span class="value">
            {sessionLength}
        </span>
    </div>

    <div class="item">
        <span class="label"> Unlocks </span>

        <span class="value">
            {unlocks}
        </span>
    </div>
</div>

<style>
    .allowance {
        display: grid;
        grid-template-columns: repeat(3, minmax(65px, 1fr));
        gap: var(--space-1);
        /* min-width: 100px; */
    }

    .item {
        display: flex;
        flex-direction: column;
    }

    .label {
        font-size: 0.75rem;
        color: var(--color-text-muted);
    }

    .value {
        font-size: 0.9rem;
        font-weight: 600;
    }

    .extension .value {
        color: var(--color-brand-light);
    }
</style>
