<script lang="ts">
    import Card from "../ui/Card.svelte";

    import type { Agent } from "../../models/agents/Agent";
    import { AgentStatus } from "../../models/agents/Agent";

    type Props = {
        agents: Agent[];
        lastRefresh: Date;
    };
    const { agents, lastRefresh }: Props = $props();

    const online = $derived(
        agents.filter((a) => a.status === AgentStatus.Online).length,
    );

    const disabled = $derived(0);

    const resources = $derived([
        ...new Set(
            agents.flatMap((agent) => agent.resources.map((r) => r.name)),
        ),
    ]);

    // Placeholder until wards/users exist.
    const users = ["Sarah", "Marcus", "Jamie"];

    // Placeholder until devices exist.
    const devices = [
        "Engineering Laptop Pool",
        "Production Server",
        "AWS Infrastructure",
    ];

    // DRY up with the date formatter in AgentCard?
    function formatLastRefresh(date: Date): string {
        const seconds = Math.floor((Date.now() - date.getTime()) / 1000);

        if (seconds < 5) return "Just now";
        if (seconds < 60) return `${seconds} seconds ago`;

        const minutes = Math.floor(seconds / 60);
        if (minutes === 1) return "1 minute ago";
        if (minutes < 60) return `${minutes} minutes ago`;

        const hours = Math.floor(minutes / 60);
        if (hours === 1) return "1 hour ago";

        return `${hours} hours ago`;
    }
</script>

<Card title="Summary">
    <div class="summary">
        <div class="metric">
            <strong>{online} / {agents.length}</strong>
            <span>Agents Online</span>
        </div>

        <div class="metric">
            <strong class="disabled">{disabled}</strong>
            <span>Agents Disabled</span>
        </div>

        <div class="management-group">
            <h4>Managed Devices</h4>

            <ul>
                {#each devices as device}
                    <li>{device}</li>
                {/each}
            </ul>
        </div>

        <div class="management-group">
            <h4>Managed Users</h4>

            <ul>
                {#each users as user}
                    <li>{user}</li>
                {/each}
            </ul>
        </div>
    </div>

    <div class="refresh">
        {formatLastRefresh(lastRefresh)}
    </div>
</Card>

<style>
    .metric {
        padding-bottom: var(--space-4);
    }

    .metric strong {
        display: block;
        font-size: 1.75rem;
        font-weight: 600;
    }

    .metric span {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .disabled {
        color: var(--color-warning);
    }

    .summary {
        display: grid;
        grid-template-columns: 1fr 1fr 1.5fr 1.5fr;
        gap: var(--space-7);

        margin-left: var(--space-6);

        align-items: center;
    }

    .management-group h4 {
        margin: 0 0 var(--space-2);

        font-size: 0.8rem;
        text-transform: uppercase;
        letter-spacing: 0.05em;

        color: var(--color-text-muted);
    }

    ul {
        margin: 0;
        padding-left: var(--space-5);

        position: relative;
        max-height: 9vh;
        overflow-y: auto;
    }

    li {
        margin-bottom: var(--space-1);
    }

    .refresh {
        margin-top: var(--space-2);
        text-align: right;
        color: var(--color-text-muted);
        font-size: 0.8rem;
    }
</style>
