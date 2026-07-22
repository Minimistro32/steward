<script lang="ts">
    let { title = "", subtitle = null } = $props();
</script>

<h1>{title}</h1>
{#if subtitle}
    <p class="text-muted">
        {@render subtitle()}
    </p>
{/if}

<style>
    h1 {
        margin-bottom: var(--space-1);
    }

    p {
        margin-bottom: var(--margin-bottom, var(--space-6));
        font-size: 0.9rem;
    }
</style>
