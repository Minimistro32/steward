<script lang="ts">
    import UserCard from "../components/users/UserCard.svelte";
    import DeviceInventory from "../components/users/DeviceInventory.svelte";

    import { getUsers } from "../api/wardApi";
    import type { User } from "../models/wards";

    import { getAgents } from "../api/agentApi";
    import type { Agent } from "../models/agents";

    let users = $state<User[]>([]);
    let agents = $state<Agent[]>([]);

    async function load() {
        [users, agents] = await Promise.all([getUsers(), getAgents()]);
    }

    load();

    function getUserSelection(user: User, agentId: string) {
        return (user.agentSelections[agentId] ??= {
            deviceIds: [],
        });
    }

    function assignDevice(user: User, agentId: string, deviceId: string) {
        const selection = getUserSelection(user, agentId);

        if (selection.deviceIds.includes(deviceId)) {
            return;
        }

        selection.deviceIds = [...selection.deviceIds, deviceId];

        users = [...users];

        console.log("Assigned", deviceId, "to", user.name);

        // Later:
        // await api.assignDevice(user.id, agentId, deviceId)
    }

    function removeDevice(user: User, agentId: string, deviceId: string) {
        const selection = getUserSelection(user, agentId);

        selection.deviceIds = selection.deviceIds.filter(
            (id) => id !== deviceId,
        );

        if (selection.deviceIds.length === 0) {
            delete user.agentSelections[agentId];
        }

        users = [...users];

        // Later:
        // await api.removeDevice(...)
    }
</script>

<div class="workspace">
    <div class="users">
        {#each users as user (user.id)}
            <UserCard
                {user}
                {agents}
                onAssign={assignDevice}
                onRemove={removeDevice}
            />
        {/each}
    </div>

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
