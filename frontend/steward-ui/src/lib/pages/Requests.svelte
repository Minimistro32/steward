<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import EmptyState from "../components/ui/EmptyState.svelte";
    import AccessOptionCard from "../components/requests/AccessOptionCard.svelte";

    import { getAccessOptions } from "../api";
    import type { AccessOption } from "../models";

    let options = $state<AccessOption[]>([]);
    let loading = $state(true);

    onMount(async () => {
        options = await getAccessOptions(1);
        loading = false;
    });
</script>

<PageHeader title="Requests">
    {#snippet subtitle()}
        Request access to your managed devices and resources.
    {/snippet}
</PageHeader>

{#if loading}
    <p>Loading access...</p>
{:else if options.length === 0}
    <EmptyState
        icon="clock.svg"
        title="Nothing to Access"
        description="There is nothing currently managed by Steward available for you to request."
    />
{:else}
    <div class="access-grid">
        {#each options as option}
            <AccessOptionCard {option} />
        {/each}
    </div>
{/if}

<style>
    .access-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(405px, 1fr));

        gap: var(--space-6);
    }
</style>
