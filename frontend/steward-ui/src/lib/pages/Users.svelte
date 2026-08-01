<script lang="ts">
    import UserCard from "../components/users/UserCard.svelte";
    import DeviceInventory from "../components/users/DeviceInventory.svelte";

    import { getUsers, getDevices } from "../api/wardApi";

    import type { User } from "../models/wards/User";
    import type { Device } from "../models/wards/Device";

    let users = $state<User[]>([]);
    let devices = $state<Device[]>([]);

    async function load() {
        users = await getUsers();
        devices = await getDevices();
    }

    load();

    function assignDevice(user: User, device: Device) {
        if (user.deviceIds.includes(device.id)) {
            return;
        }

        user.deviceIds = [...user.deviceIds, device.id];

        users = [...users];

        console.log("Assigned", device.name, "to", user.name);

        // Later:
        // await api.assignDevice(user.id, device.id)
    }

    function removeDevice(user: User, deviceId: string) {
        user.deviceIds = user.deviceIds.filter((id) => id !== deviceId);

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
                {devices}
                onAssign={assignDevice}
                onRemove={removeDevice}
            />
        {/each}
    </div>

    <div class="inventory">
        <DeviceInventory {devices}/>
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
