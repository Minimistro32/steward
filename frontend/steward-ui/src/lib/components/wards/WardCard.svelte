<script lang="ts">
    import Card from "../ui/Card.svelte";
    import type { Ward } from "../../models/wards/Ward";

    type Props = {
        ward: Ward;
        selected: boolean;
        onclick: () => void;
    };

    const { ward, selected, onclick }: Props = $props();
</script>

<div class:selected>
    <Card>
        {#snippet actions()}
            <button
                class="menu"
                onclick={(event) => {
                    event.stopPropagation();
                    // TODO: Open agent actions menu
                }}
                aria-label="Agent actions"
            >
                ⋮
            </button>
        {/snippet}

        <button class="card-button" {onclick}>
            <h2>{ward.name}</h2>

            <div class="summary">
                <span>{ward.users.length} Users</span>
                <span>{ward.devices.length} Devices</span>
                <span>{ward.resources.length} Resources</span>
            </div>
        </button>
    </Card>
</div>

<style>
    .card-button {
        width: 100%;
        text-align: left;

        background: none;
        border: none;
        color: inherit;

        cursor: pointer;
        padding: 0;
    }

    h1 {
        margin: 0 0 var(--space-4);
    }

    .summary {
        display: flex;
        gap: var(--space-4);

        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .selected {
        outline: 2px solid var(--color-brand);
        border-radius: var(--radius-md);
    }

    .menu {
        width: 32px;
        height: 32px;

        background: transparent;
        border: none;
        border-radius: var(--radius-sm);

        color: var(--color-text-muted);

        cursor: pointer;
    }

    .menu:hover {
        background: var(--color-surface-raised);
        color: var(--color-text);
    }
</style>
