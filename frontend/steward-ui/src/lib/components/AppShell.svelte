<script lang="ts">
    import Sidebar from "./Sidebar.svelte";
    import TopBar from "./TopBar.svelte";

    let { children } = $props();
</script>

<div class="shell">
    <Sidebar />

    <div class="content">
        <TopBar />

        <main>
            {@render children()}
        </main>
    </div>
</div>

<style>
    .shell {
        display: flex;
        min-height: 100vh;
        background: var(--color-background);
    }

    .content {
        flex: 1;
        display: flex;
        flex-direction: column;
    }

    main {
        flex: 1;
        padding: var(--space-6) var(--space-8);
    }
</style>
