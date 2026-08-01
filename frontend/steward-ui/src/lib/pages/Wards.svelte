<script lang="ts">
    import PageHeader from "../components/ui/PageHeader.svelte";
    import Card from "../components/ui/Card.svelte";
    import CurrentWard from "../components/wards/CurrentWard.svelte";
    import WardCard from "../components/wards/WardCard.svelte";
    import { type Ward } from "../models/wards/Ward";

    const wards: Ward[] = [
        {
            id: "alice",
            name: "Alice",
            tags: ["gaming", "school"],

            users: [
                {
                    id: "alice",
                    name: "Alice",
                },
            ],

            devices: [
                {
                    id: "gaming-pc",
                    name: "Gaming PC",
                },
                {
                    id: "laptop",
                    name: "Laptop",
                },
            ],

            resources: ["Steam", "Discord", "YouTube"],
        },

        {
            id: "kids",
            name: "Kids Devices",
            tags: ["family"],

            users: [
                {
                    id: "alice",
                    name: "Alice",
                },
                {
                    id: "bob",
                    name: "Bob",
                },
            ],

            devices: [
                {
                    id: "desktop",
                    name: "Family Desktop",
                },
                {
                    id: "switch",
                    name: "Nintendo Switch",
                },
            ],

            resources: ["Internet", "Minecraft", "Discord"],
        },

        {
            id: "gaming",
            name: "Gaming Consoles",
            tags: [],

            users: [],

            devices: [
                {
                    id: "xbox",
                    name: "Xbox",
                },
                {
                    id: "ps5",
                    name: "PS5",
                },
            ],

            resources: ["Power", "Network"],
        },
    ];

    $: columns = [
        wards.filter((_, i) => i % 5 === 0),
        wards.filter((_, i) => i % 5 === 1),
        wards.filter((_, i) => i % 5 === 2),
        wards.filter((_, i) => i % 5 === 3),
        wards.filter((_, i) => i % 5 === 4),
    ];

    let selectedWard = wards[0];

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

<div class="currentWard">
    <CurrentWard ward={selectedWard} />
</div>

<!-- <Card>
    Users -> Devices x Resources
</Card> -->

<h2>Wards</h2>

<div class="masonry">
    {#each columns as column}
        <div class="column">
            {#each column as ward (ward.id)}
                <WardCard
                    {ward}
                    selected={selectedWard.id === ward.id}
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
