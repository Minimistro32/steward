<script lang="ts">
    import { currentUser } from "../session";
    import Sidebar from "./Sidebar.svelte";
    import TopBar from "./TopBar.svelte";

    let { children } = $props();
</script>

<div class="shell">
    {#if $currentUser?.type === "admin"}<Sidebar />{/if}

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
        min-width: 0;
        flex: 1;
        display: flex;
        flex-direction: column;
    }

    main {
        flex: 1;
        padding: var(--space-6) var(--space-8);
    }
    @media (max-width: 600px) { main { padding: var(--space-4); } }
</style>
