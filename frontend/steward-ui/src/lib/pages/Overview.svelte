<script lang="ts">
    import PageHeader from "../components/PageHeader.svelte";
    import AgentHealth from "../components/widgets/AgentHealth.svelte";
    import Enforcement from "../components/widgets/Enforcement.svelte";
    import RequestActivity from "../components/widgets/RequestActivity.svelte";
    import Timeline from "../components/widgets/Timeline.svelte";
</script>

<PageHeader title="Overview">
    {#snippet subtitle()}
        <a href="#/agents">Agents</a> tend each <a href="#/wards">ward</a>
        with the <a href="#/policies">policies</a> you define.
    {/snippet}
</PageHeader>

<div class="dashboard-grid">
    <AgentHealth />
    <Enforcement --row-span="1" />
    <RequestActivity --row-span="2" />
    <!-- <Timeline --col-span="1" /> -->
    <Timeline --col-span="2" />
</div>

<!-- <StatusBadge status="online"/> -->

<style>
    a {
        font-size: 1rem;
        text-decoration: none;
        color: var(--color-text-muted);
        font-weight: bold;
    }

    a:hover {
        color: var(--color-brand-light);
        text-decoration: underline;
    }

    .dashboard-grid {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: var(--space-5);
    }
</style>
