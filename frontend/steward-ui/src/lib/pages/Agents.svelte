<script lang="ts">
    import { onMount } from "svelte";

    import AgentCard from "../components/agents/AgentCard.svelte";
    import AgentSummary from "../components/agents/AgentSummary.svelte";
    import PageHeader from "../components/ui/PageHeader.svelte";

    import type { Agent, User } from "../models";
    import { getAgents, refreshAgents, getUsers } from "../api";
    import EmptyState from "../components/ui/EmptyState.svelte";

    let agents = $state<Agent[]>([]);
    let users = $state<User[]>([]);

    async function loadAgents() {
        [agents, users] = await Promise.all([getAgents(), getUsers()]);
    }

    onMount(async () => {
        await loadAgents();
    });

    type AgentCardData = {
        agent: Agent;
        users: User[];
    };

    const cardData = $derived(
        agents.map((agent): AgentCardData => {
            const agentDeviceIds = new Set(
                agent.devices.map((device) => device.id),
            );

            const cardUsers = users.filter((user) =>
                user.deviceIds.some((deviceId) => agentDeviceIds.has(deviceId)),
            );

            return {
                agent,
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
        await refreshAgents();
        // agents = getAgents;
        lastRefresh = new Date();
    }
</script>

<PageHeader title="Agents">
    {#snippet subtitle()}
        Agents watch over wards by enforcing policies.
    {/snippet}
    {#snippet actions()}
        <button class="cta-button" onclick={refresh}> ↻ Refresh </button>
    {/snippet}
</PageHeader>

<AgentSummary {agents} {users} {lastRefresh} />

<h2>Registered Agents</h2>

{#if agents.length === 0}
    <EmptyState
        icon="server-stack.svg"
        title="No Agents"
        description="Connect a Steward agent via MQTT and hit refresh."
    ></EmptyState>
{:else}
    <div class="masonry">
        {#each columns as column}
            <div class="column">
                {#each column as data (data.agent.agentId)}
                    <AgentCard {data} />
                {/each}
            </div>
        {/each}
    </div>
{/if}

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
