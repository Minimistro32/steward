<script lang="ts">
    import AgentHealth from "../components/widgets/AgentHealth.svelte";
    import Enforcement from "../components/widgets/Enforcement.svelte";
    import RequestActivity from "../components/widgets/RequestActivity.svelte";
    import Timeline from "../components/widgets/Timeline.svelte";
</script>

<h1>Dashboard</h1>

<p class="text-muted">Welcome to Steward.</p>

<div class="dashboard-grid">
    <AgentHealth />
    <Enforcement --row-span="1" />
    <RequestActivity --row-span="2" />
    <!-- <Timeline --col-span="1" /> -->
    <Timeline --col-span="2" />
</div>

<!-- <StatusBadge status="online"/> -->

<style>
    h1 {
        margin-bottom: 0.6rem;
    }

    p {
        margin-bottom: 1.5rem;
    }

    .dashboard-grid {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: var(--space-5);
    }
</style>
