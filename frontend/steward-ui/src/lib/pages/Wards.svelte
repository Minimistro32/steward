<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import CurrentWard from "../components/wards/CurrentWard.svelte";
    import WardCard from "../components/wards/WardCard.svelte";

    import { type Ward } from "../models/wards/Ward";
    import { getWards } from "../api/wardApi";

    let wards = $state<Ward[]>([]);
    let selectedWard = $state<Ward | undefined>(undefined);
    let loading = $state(true);

    onMount(async () => {
        wards = await getWards();

        if (wards.length > 0) {
            selectedWard = wards[0];
        }

        loading = false;
    });

    const columns = $derived([
        wards.filter((_, i) => i % 5 === 0),
        wards.filter((_, i) => i % 5 === 1),
        wards.filter((_, i) => i % 5 === 2),
        wards.filter((_, i) => i % 5 === 3),
        wards.filter((_, i) => i % 5 === 4),
    ]);

    function selectWard(ward: Ward) {
        selectedWard = ward;
    }
</script>

<PageHeader title="Wards">
    {#snippet subtitle()}
        Group users, devices, and resources into reusable policy targets.
    {/snippet}

    {#snippet actions()}
        <a href="#/wards/new">
            <button class="cta-button"> + Create Ward </button>
        </a>
    {/snippet}
</PageHeader>

{#if loading}
    <p>Loading wards...</p>
{:else if selectedWard}
    <div class="currentWard">
        <CurrentWard ward={selectedWard} />
    </div>
{/if}

<h2>Wards</h2>

<div class="masonry">
    {#each columns as column}
        <div class="column">
            {#each column as ward (ward.id)}
                <WardCard
                    {ward}
                    selected={selectedWard?.id === ward.id}
                    onclick={() => selectWard(ward)}
                />
            {/each}
        </div>
    {/each}
</div>

<style>
    h2 {
        margin: var(--space-7) 0 var(--space-4);
    }

    .masonry {
        display: grid;
        grid-template-columns: repeat(5, 1fr);
        gap: var(--space-6);
        align-items: start;
    }

    .column {
        display: flex;
        flex-direction: column;
        gap: var(--space-6);
    }
</style>
