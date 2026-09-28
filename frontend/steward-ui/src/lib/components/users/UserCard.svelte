<script lang="ts">
    import { currentUser } from "../../session";
    import Card from "../ui/Card.svelte";

    import type { User, Device } from "../../models";

    type Props = {
        user: User;
        devices: Device[];
        onAssign: (user: User, deviceId: number) => void;
        onRemove: (user: User, deviceId: number) => void;
    };

    const { user, devices, onAssign, onRemove }: Props = $props();

    const canEdit = $derived($currentUser?.type === "admin" && (user.type !== "admin" || user.id === $currentUser.id));

    const assignedDevices = $derived.by(() =>
        user.deviceIds
            .map((id) => devices.find((d) => d.id === id))
            .filter((device): device is Device => device !== undefined),
    );

    function drop(event: DragEvent) {
        event.preventDefault();
        if (!canEdit) return;

        const deviceId = Number(event.dataTransfer?.getData("deviceId"));
        if (deviceId) {
            onAssign(user, deviceId);
        }
    }

    function dragOver(event: DragEvent) {
        event.preventDefault();

        if (event.dataTransfer) {
            event.dataTransfer.dropEffect = canEdit ? "copy" : "none";
        }
    }
</script>

<Card>
    <div
        class="card"
        role="region"
        aria-label={`Devices assigned to ${user.name}`}
        ondragover={dragOver}
        ondrop={drop}
    >
        <div class="header">
            <h2>
                {user.name}
            </h2>

            {#if canEdit}
            <a class="edit-user" href={`#/users/${user.id}`} aria-label={`Edit ${user.name}`} title="Edit user">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                    <circle cx="5" cy="12" r="2" />
                    <circle cx="12" cy="12" r="2" />
                    <circle cx="19" cy="12" r="2" />
                </svg>
            </a>
            {/if}
        </div>

        <div class="devices">
            {#if assignedDevices.length === 0}
                <p class="empty">{canEdit ? "Drop devices here" : "No devices assigned"}</p>
            {:else}
                {#each assignedDevices as device}
                    <div class="device-chip">
                        <span>
                            {device.name}
                        </span>

                        {#if canEdit}
                        <button
                            onclick={() => onRemove(user, device.id)}
                            aria-label="Remove device"
                        >
                            &#215;
                        </button>
                        {/if}
                    </div>
                {/each}
            {/if}
        </div>
        <div class="device-count">
            {assignedDevices.length} device{assignedDevices.length === 1 ? "" : "s"}
        </div>
    </div>
</Card>

<style>
    .card {
        min-height: 120px;
        display: flex;
        flex-direction: column;
    }

    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;

        margin-bottom: var(--space-4);
    }

    h2 {
        margin: 0;
    }

    .edit-user {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        width: 44px;
        height: 44px;
        margin-left: var(--space-3);
        border-radius: var(--radius-sm);
        color: var(--color-text-muted);
    }

    .edit-user:hover {
        color: var(--color-text);
        background: var(--color-surface-raised);
    }

    .edit-user:focus-visible {
        outline: 2px solid var(--color-brand-light);
        outline-offset: 2px;
    }

    .device-count {
        align-self: flex-end;
        margin-top: auto;
        padding-top: var(--space-3);
        color: var(--color-text-muted);
        font-size: 0.8rem;
    }

    .devices {
        display: flex;
        flex-wrap: wrap;

        gap: var(--space-2);

        min-height: 40px;

        padding: var(--space-3);

        border: 1px dashed var(--color-border);
        border-radius: var(--radius-md);
    }

    .device-chip {
        display: flex;
        align-items: center;

        gap: var(--space-2);

        background: var(--color-surface-raised);

        padding: var(--space-2) var(--space-3);

        border-radius: var(--radius-sm);

        font-size: 0.85rem;
    }

    button {
        border: none;
        background: transparent;

        color: var(--color-text-muted);

        cursor: pointer;

        font-size: 1rem;
        line-height: 1;
    }

    button:hover {
        color: var(--color-danger);
    }

    .empty {
        margin: 0;

        width: 100%;

        text-align: center;

        color: var(--color-text-muted);

        font-size: 0.85rem;
    }
</style>
