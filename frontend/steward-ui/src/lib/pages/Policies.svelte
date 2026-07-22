<script lang="ts">
    import PageHeader from "../components/ui/PageHeader.svelte";
    import ConfigurationSummary from "../components/policies/ConfigurationSummary.svelte";
    import StatusDot from "../components/ui/StatusDot.svelte";

    type Policy = {
        name: string;
        ward: string;
        active: boolean;
        schedule: string;
        access: {
            dailyTime: string;
            sessionLength: string;
            unlocks: string;
        };
        override?: string;
        allowance?: {
            dailyTime: string;
            sessionLength: string;
            unlocks: string;
        };
    };

    let policies: Policy[] = [
        {
            name: "Gaming Restrictions",
            ward: "Alice",
            active: true,
            schedule: "Weekdays 8PM - 7AM",
            access: {
                dailyTime: "60 min",
                sessionLength: "30 min",
                unlocks: "3",
            },
            override: "Escalating Delay",
            allowance: {
                dailyTime: "+30 min",
                sessionLength: "15 min",
                unlocks: "+1",
            },
        },
        {
            name: "School Night Rules",
            ward: "Kids Devices",
            active: true,
            schedule: "Sun - Thu",
            access: {
                dailyTime: "90 min",
                sessionLength: "45 min",
                unlocks: "2",
            },
        },
        {
            name: "Weekend Relaxed",
            ward: "Bob",
            active: false,
            schedule: "Saturday",
            access: {
                dailyTime: "∞",
                sessionLength: "60 min",
                unlocks: "5",
            },
            override: "User Approval",
            allowance: {
                dailyTime: "+60 min",
                sessionLength: "30 min",
                unlocks: "+1",
            },
        },
    ];
</script>

<PageHeader title="Policies">
    {#snippet subtitle()}
        Define how wards access their resources and request additional access.
    {/snippet}
</PageHeader>

<div class="toolbar">
    <a href="#/policyform">
        <button class="cta-button"> + Create Policy </button>
    </a>
</div>

<div class="table-container">
    <table>
        <thead>
            <tr>
                <th> Name </th>
                <th> Ward </th>
                <th> Schedule </th>
                <th> Access </th>
                <th> Override </th>
                <th> Allowing </th>
                <th></th>
            </tr>
        </thead>

        <tbody>
            {#each policies as policy}
                <tr>
                    <td class="col1">
                        <strong>
                            {policy.name}
                        </strong>

                        <div class="status">
                            <StatusDot
                                label={policy.active ? "Active" : "Disabled"}
                                color={policy.active
                                    ? "var(--color-success)"
                                    : "var(--color-text-muted)"}
                                --font-size="0.85rem"
                            />
                        </div>
                    </td>

                    <td>
                        {policy.ward}
                    </td>

                    <td>
                        {policy.schedule}
                    </td>

                    <td>
                        <ConfigurationSummary
                            dailyTime={policy.access.dailyTime}
                            sessionLength={policy.access.sessionLength}
                            unlocks={policy.access.unlocks}
                        />
                    </td>

                    <td>
                        {#if policy.override}
                            {policy.override}
                        {:else}
                            <span class="text-muted"> Disabled </span>
                        {/if}
                    </td>

                    <td>
                        {#if policy.allowance}
                            <ConfigurationSummary
                                extension
                                dailyTime={policy.allowance.dailyTime}
                                sessionLength={policy.allowance.sessionLength}
                                unlocks={policy.allowance.unlocks}
                            />
                        {/if}
                    </td>

                    <td>
                        <button class="cta-button"> Edit </button>
                    </td>
                </tr>
            {/each}
        </tbody>
    </table>
</div>

<style>
    .col1 {
        display: flex;
        flex-direction: column;
    }

    .toolbar {
        display: flex;
        justify-content: flex-end;
        margin-bottom: var(--space-6);
    }

    .table-container {
        background: var(--color-surface);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-lg);
        overflow: hidden;
    }

    table {
        width: 100%;
        border-collapse: collapse;
    }

    th {
        text-align: left;
        padding: var(--space-4);
        color: var(--color-text-muted);
        font-size: 0.85rem;
        border-bottom: 1px solid var(--color-border);
    }

    td {
        padding: var(--space-4);
        border-bottom: 1px solid var(--color-border);
    }

    tbody tr:hover {
        background: var(--color-surface-raised);
    }

    .status {
        padding-left: var(--space-6);
    }
</style>
