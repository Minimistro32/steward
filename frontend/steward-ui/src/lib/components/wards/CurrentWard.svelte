<script lang="ts">
    import Card from "../ui/Card.svelte";
    import type { Ward } from "../../models/wards/Ward";

    type Props = {
        ward: Ward;
    };

    const { ward }: Props = $props();
</script>

<Card title="Management Surface">
    <div class="relationship">
        <section>
            <h4>Users</h4>

            <ul>
                {#each ward.users as user}
                    <li>
                        {user.name}
                    </li>
                {:else}
                    <li class="empty">No users</li>
                {/each}
            </ul>
        </section>

        <div class="arrow">
            →
            <span>
                Policies apply<br />
                per user
            </span>
        </div>

        <section>
            <h4>Devices</h4>

            <ul>
                {#each ward.devices as device}
                    <li>
                        {device.name}
                    </li>
                {:else}
                    <li class="empty">No devices</li>
                {/each}
            </ul>
        </section>

        <!-- <div class="cross">&#215;</div> -->
        <div class="cross">
            &#215;
            <span>
                to every device and resource
            </span>
        </div>

        <section>
            <h4>Resources</h4>

            <ul>
                {#each ward.resources as resource}
                    <li>
                        {resource}
                    </li>
                {:else}
                    <li class="empty">No resources</li>
                {/each}
            </ul>
        </section>
    </div>

    <p class="description">
        Every listed resource is managed on every listed device. Policies are
        evaluated independently for each user.
    </p>
</Card>

<style>
    .relationship {
        display: grid;
        grid-template-columns: 14% auto 14% auto 14%;
        gap: 5%;
        align-items: start;
        justify-content: center;
        justify-items: center;
    }

    section {
        display: flex;
        flex-direction: column;
        width: max-content;
        border-width: 3px;
        border-style: outset;
        border-color: var(--color-text-muted);
        border-radius: var(--space-2);
        padding: var(--space-3);
        height: 14vh;
    }

    h4 {
        margin: 0 0 var(--space-2);
        height: 1rem;

        color: var(--color-text-muted);

        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.05em;
    }

    ul {
        margin: 0;
        padding-left: var(--space-5);
        padding-right: var(--space-5);

        position: relative;
        max-height: 10vh;
        overflow-y: auto;
    }

    li {
        margin-bottom: var(--space-1);
    }

    .arrow {
        display: flex;

        flex-direction: column;

        align-items: center;
        justify-content: center;

        color: var(--color-brand);

        font-size: 1.5rem;
        margin-top: var(--space-6);
    }

    .arrow span {
        width: 80px;

        text-align: center;

        color: var(--color-text-muted);

        font-size: 0.7rem;
        line-height: 1.2;
    }

    .cross {
        display: flex;

        flex-direction: column;

        align-items: center;
        justify-content: center;

        font-size: 2rem;

        color: var(--color-brand);

        font-weight: bold;

        margin-top: var(--space-5);
    }

    .cross span {
        width: 100px;

        text-align: center;

        color: var(--color-text-muted);

        font-size: 0.7rem;
        line-height: 1.2;
        font-weight: normal;
    }

    .description {
        margin-top: var(--space-5);

        color: var(--color-text-muted);

        text-align: center;

        font-size: 0.85rem;
    }
</style>
