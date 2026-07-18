<script lang="ts">
    import AgentCard from "../components/agents/AgentCard.svelte";
    import AgentSummary from "../components/agents/AgentSummary.svelte";
    import PageHeader from "../components/PageHeader.svelte";

    let agents = [
        {
            name: "Pi-Hole",
            status: "online",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
        {
            name: "ESP32-PS5",
            status: "online",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
        {
            name: "Cold Turkey Blocker",
            status: "offline",
            lastSeen: "3 hours ago",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
        {
            name: "Pi-Hole2",
            status: "online",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
        {
            name: "ESP32-PS52",
            status: "online",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
        {
            name: "Cold Turkey Blocker2",
            status: "offline",
            lastSeen: "3 hours ago",
            resourceList: ["GitHub", "AWS", "Jira"],
            userList: ["Sarah", "Marcus"],
            deviceList: ["ENG-LAPTOP-04", "Build Server"],
        },
    ];

    let columns = $derived([
        agents.filter((_, i) => i % 3 === 0),
        agents.filter((_, i) => i % 3 === 1),
        agents.filter((_, i) => i % 3 === 2),
    ]);
</script>

<PageHeader title="Agents">
    {#snippet subtitle()}
        Agents watch over wards, discover resources, and enforce policies.
    {/snippet}
</PageHeader>

<AgentSummary />

<h2>Registered Agents</h2>

<div class="masonry">
    {#each columns as column}
        <div class="column">
            {#each column as agent (agent.name)}
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
