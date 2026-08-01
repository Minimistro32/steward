<script lang="ts">
    import { onMount } from "svelte";

    import AgentCard from "../components/agents/AgentCard.svelte";
    import AgentSummary from "../components/agents/AgentSummary.svelte";
    import PageHeader from "../components/ui/PageHeader.svelte";

    import { type Agent } from "../models/agents";
    import type { Device, User } from "../models/wards";
    import { getAgents, refreshAgents } from "../api/agentApi";
    import { getDevices, getUsers } from "../api/wardApi";

    let agents = $state<Agent[]>([]);
    let devices = $state<Device[]>([]);
    let users = $state<User[]>([]);

    async function loadAgents() {
        agents = await getAgents();
        devices = await getDevices();
        users = await getUsers();
    }

    onMount(async () => {
        await loadAgents();
    });

    type AgentCardData = {
        agent: Agent;
        devices: Device[];
        users: User[];
    };

    const cardData = $derived(
        agents.map((agent): AgentCardData => {
            const cardDevices = devices.filter(
                (device) => device.agentId === agent.agentId,
            );

            const cardUsers = users.filter((user) =>
                user.deviceIds.some((userDevice) =>
                    cardDevices.some((device) => device.id === userDevice),
                ),
            );

            return {
                agent,
                devices: cardDevices,
                users: cardUsers,
            };
        }),
    );

    const columns = $derived([
        cardData.filter((_, i) => i % 3 === 0),
        cardData.filter((_, i) => i % 3 === 1),
        cardData.filter((_, i) => i % 3 === 2),
    ]);

    let lastRefresh = $state(new Date());

    async function refresh() {
        agents = await refreshAgents();
        lastRefresh = new Date();
    }
</script>

<PageHeader title="Agents">
    {#snippet subtitle()}
        Agents watch over wards, discover resources, and enforce policies.
    {/snippet}
    {#snippet actions()}
        <button class="cta-button" onclick={refresh}> ↻ Refresh </button>
    {/snippet}
</PageHeader>

<AgentSummary {agents} {devices} {users} {lastRefresh} />

<h2>Registered Agents</h2>

<div class="masonry">
    {#each columns as column}
        <div class="column">
            {#each column as data (data.agent.agentId)}
                <AgentCard {data} />
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
        grid-template-columns: repeat(3, 1fr);
        gap: var(--space-6);
        align-items: start;
    }

    .column {
        display: flex;
        flex-direction: column;
        gap: var(--space-6);
    }
</style>
