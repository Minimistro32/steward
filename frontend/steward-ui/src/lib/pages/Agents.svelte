<script lang="ts">
    import AgentCard from "../components/agents/AgentCard.svelte";
    import AgentSummary from "../components/agents/AgentSummary.svelte";
    import PageHeader from "../components/ui/PageHeader.svelte";

    import type { Agent } from "../models/agents/Agent";
    import { getAgents, refreshAgents } from "../api/mockAgentsApi";

    let agents: Agent[] = getAgents();

    $: columns = [
        agents.filter((_, i) => i % 3 === 0),
        agents.filter((_, i) => i % 3 === 1),
        agents.filter((_, i) => i % 3 === 2),
    ];

    let lastRefresh = new Date();

    function refresh() {
        agents = refreshAgents();
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

<AgentSummary
    agents={agents}
    lastRefresh={lastRefresh}
/>

<h2>Registered Agents</h2>

<div class="masonry">
    {#each columns as column}
        <div class="column">
            {#each column as agent (agent.agentId)}
                <AgentCard {agent} />
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
