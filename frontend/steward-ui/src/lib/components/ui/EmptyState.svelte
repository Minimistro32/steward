<script lang="ts">
    import type { Snippet } from "svelte";
    import Card from "./Card.svelte";

    let {
        icon = "/steward-logo.svg",
        title,
        description,
        actions,
    }: {
        icon?: string;
        title: string;
        description: string;
        actions?: Snippet;
    } = $props();
</script>

<Card>
    <div class="empty-state">
        <img class="icon" src={icon} alt="Icon showing empty state" />

        <h2>{title}</h2>

        <p>{description}</p>

        {#if actions}
            <div class="actions">
                {@render actions()}
            </div>
        {/if}
    </div>
</Card>

<style>
    .empty-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;

        padding: var(--space-7) var(--space-7);

        text-align: center;
    }

    .icon {
        width: var(--space-10);
        margin-bottom: var(--space-4);
    }

    h2 {
        margin: 0 0 var(--space-2);
    }

    p {
        max-width: 32rem;
        color: var(--color-text-muted);
        margin: 0;
    }

    .actions {
        display: flex;
        gap: var(--space-3);
    }
</style>
