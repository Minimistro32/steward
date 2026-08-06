<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import EmptyState from "../components/ui/EmptyState.svelte";
    import AccessOptionCard from "../components/requests/AccessOptionCard.svelte";

    import { getAccessOptions, getUsers } from "../api";
    import type { AccessOption, User } from "../models";

    let selectedUserId = $state<number | undefined>(undefined);
    let users = $state<User[]>([]);
    let options = $state<AccessOption[]>([]);
    let loadingUsers = $state(true);
    let loadingOptions = $state(false);

    onMount(async () => {
        users = await getUsers();
        loadingUsers = false;
    });

    $effect(() => {
        if (!selectedUserId) {
            options = [];
            return;
        }

        loadOptions(selectedUserId);
    });

    async function loadOptions(userId: number) {
        loadingOptions = true;

        try {
            options = await getAccessOptions(userId);
            options[0].state = "Unavailable";
        } finally {
            loadingOptions = false;
        }
    }
</script>

<PageHeader title="Requests">
    {#snippet subtitle()}
        Request access to your managed devices and resources.
    {/snippet}
    {#snippet actions()}
        {#if loadingUsers}
            <p>Loading users...</p>
        {:else}
            <select
                bind:value={selectedUserId}
                class:placeholder={!selectedUserId}
            >
                <option value={undefined} disabled hidden>Select a user</option>
                {#each users as user}
                    <option value={user.id}>
                        {user.name}
                    </option>
                {/each}
            </select>
        {/if}
    {/snippet}
</PageHeader>

{#if selectedUserId === undefined}
    <EmptyState
        icon="user-circle.svg"
        title="Select a User"
        description="Select a user to view what is available to request."
    />
{:else if loadingOptions}
    <p>Loading options...</p>
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
