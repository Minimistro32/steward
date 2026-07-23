<script lang="ts">
    let { title = "", subtitle = null, actions = null } = $props();
</script>

<div class="page-header">
    <div>
        <h1>{title}</h1>
        {#if subtitle}
            <p class="text-muted">
                {@render subtitle()}
            </p>
        {/if}
    </div>

    {#if actions}
        <div class="action">
            {@render actions()}
        </div>
    {/if}
</div>

<style>
    .page-header {
        display: flex;
        columns: 2;
        justify-content: space-between;
    }

    h1 {
        margin-bottom: var(--space-1);
    }

    p {
        margin-bottom: var(--margin-bottom, var(--space-6));
        font-size: 0.9rem;
    }

    .action {
        height: var(--space-8);
        align-self: center;
    }
</style>
