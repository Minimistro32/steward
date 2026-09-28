<script lang="ts">
    import { currentUser } from "../session";
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import EmptyState from "../components/ui/EmptyState.svelte";
    import RequestActivity from "../components/overview/RequestActivity.svelte";
    import AccessOptionCard from "../components/requests/AccessOptionCard.svelte";
    import AccessRequestDialog from "../components/requests/AccessRequestDialog.svelte";

    import { getAccessOptions, getUsers } from "../api";
    import type { AccessOption, User } from "../models";

    let selectedUserId = $state<number | undefined>(undefined);
    let users = $state<User[]>([]);
    let options = $state<AccessOption[]>([]);
    let loadingUsers = $state(true);
    let loadingOptions = $state(false);

    // The option currently being requested.
    // null means the dialog is closed.
    let selectedOption = $state<AccessOption | null>(null);


    onMount(async () => {
        if ($currentUser?.type === "admin") users = await getUsers();
        else selectedUserId = $currentUser?.id;
        loadingUsers = false;
    });

    $effect(() => {
        if (!selectedUserId) {
            options = [];
            selectedOption = null;
            return;
        }

        loadOptions(selectedUserId);
    });

    async function loadOptions(userId: number) {
        loadingOptions = true;

        try {
            options = await getAccessOptions(userId);
        } finally {
            loadingOptions = false;
        }
    }


    function openRequestDialog(option: AccessOption) {
        selectedOption = option;
    }


    function closeRequestDialog() {
        selectedOption = null;
    }


    async function requestCompleted() {
        if (selectedUserId === undefined) {
            return;
        }

        await loadOptions(selectedUserId);
    }
</script>

<PageHeader title="Requests">
    {#snippet subtitle()}
        Request access to your managed devices and resources.
    {/snippet}
    {#snippet actions()}
        {#if $currentUser?.type !== "admin"}
            <span>{$currentUser?.name}</span>
        {:else if loadingUsers}
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

<div class="requests-layout">
    <div class="request-options">
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
                    <AccessOptionCard
                        {option}
                        onclick={openRequestDialog}
                    />
                {/each}
            </div>

        {/if}
    </div>

    <div class="request-activity">
        <RequestActivity showMakeRequest={false} />
    </div>
</div>

{#if selectedUserId !== undefined && selectedOption !== null}

    <AccessRequestDialog
        userId={selectedUserId}
        option={selectedOption}
        onclose={closeRequestDialog}
        oncomplete={requestCompleted}
    />

{/if}

<style>
    .requests-layout {
        display: grid;
        grid-template-columns: minmax(0, 1fr) minmax(325px, min(28%, 400px));
        align-items: start;
        gap: var(--space-6);
    }

    .request-options,
    .request-activity {
        min-width: 0;
    }

    .access-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(min(405px, 100%), 1fr));
        justify-items: center;

        gap: var(--space-6);
    }
    @media (max-width: 1000px) {
        .requests-layout {
            grid-template-columns: minmax(0, 1fr);
        }

        .request-activity {
            grid-row: 1;
            width: 100%;
            max-width: 550px;
            justify-self: center;
            --request-activity-height: min(25vh, 280px);
            --request-activity-empty-min-height: 140px;
        }
    }
</style>
