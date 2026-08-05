<script lang="ts">
    import UserCard from "../components/users/UserCard.svelte";
    import DeviceInventory from "../components/users/DeviceInventory.svelte";
    import EmptyState from "../components/ui/EmptyState.svelte";
    import PageHeader from "../components/ui/PageHeader.svelte";

    import {
        getUsers,
        getAgents,
        assignUserDevice,
        removeUserDevice,
    } from "../api";
    import type { User, Agent } from "../models";

    let users = $state<User[]>([]);
    let agents = $state<Agent[]>([]);

    async function load() {
        [users, agents] = await Promise.all([getUsers(), getAgents()]);
    }

    load();

    const devices = $derived(agents.flatMap((agent) => agent.devices));

    function assignDevice(user: User, deviceId: number) {
        if (user.deviceIds.includes(deviceId)) {
            return;
        }

        user.deviceIds = [...user.deviceIds, deviceId];

        users = [...users];

        console.log("Assigned", deviceId, "to", user.name);

        assignUserDevice(user.id, deviceId);
    }

    function removeDevice(user: User, deviceId: number) {
        user.deviceIds = user.deviceIds.filter((id) => id !== deviceId);

        users = [...users];

        console.log("Removed", deviceId, "from", user.name);

        removeUserDevice(user.id, deviceId);
    }

    
</script>

<PageHeader title="Users">
    {#snippet subtitle()}
        Users are assigned the devices they use and added to wards.
    {/snippet}
    {#snippet actions()}
        <button class="cta-button"> + Create User </button>
    {/snippet}
</PageHeader>

<div class="workspace">
    {#if users.length === 0}
        <EmptyState
            icon="users.svg"
            title="No Users"
            description="Create a user to begin assigning devices and managing screen time."
        />
    {:else}
        <div class="users">
            {#each users as user (user.id)}
                <UserCard
                    {user}
                    {devices}
                    onAssign={assignDevice}
                    onRemove={removeDevice}
                />
            {/each}
        </div>
    {/if}

    <div class="inventory">
        <DeviceInventory {agents} />
    </div>
</div>

<style>
    .workspace {
        display: grid;
        grid-template-columns: minmax(0, 1fr) 320px;

        gap: var(--space-6);

        align-items: start;
    }

    .users {
        display: flex;
        flex-direction: column;

        gap: var(--space-6);
    }

    .inventory {
        position: sticky;
        top: var(--space-6);
    }
</style>
