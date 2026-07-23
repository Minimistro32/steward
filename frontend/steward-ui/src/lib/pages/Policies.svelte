<script lang="ts">
    import PageHeader from "../components/ui/PageHeader.svelte";
    import AllowanceSummary from "../components/policies/AllowanceSummary.svelte";
    import StatusDot from "../components/ui/StatusDot.svelte";

    import type { OverrideRequirement } from "../models/policies/OverridePolicy";
    import { getPolicies } from "../api/mockPoliciesApi";
    import { formatTimeRange } from "../models/policies/Schedule";

    const policies = getPolicies();
    const wards: Record<string, string> = {
        alice: "Alice",
        kids: "Kids Devices",
        bob: "Bob",
    };

    function minutes(value?: number) {
        return value == null ? "∞" : `${value} min`;
    }

    function unlocks(value?: number) {
        return value == null ? "∞" : value.toString();
    }

    function overrideLabel(
        requirement: OverrideRequirement | undefined,
    ): string | undefined {
        if (requirement === "userApproval") return "User Approval";
        if (requirement === "randomText") return "Random Text";
        if (requirement === "delay") return "Delay";

        return requirement;
    }

    // scheduleSummary
    import type { Schedule } from "../models/policies/Schedule";

    const dayNames = [
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
    ];

    export function scheduleSummary(schedule: Schedule): {
        days: string;
        time: string | undefined;
    } {
        const days = [...schedule.days].sort((a, b) => a - b);

        const timeText = formatTimeRange(schedule);
        let dayText: string;

        if (arraysEqual(days, [0, 1, 2, 3, 4, 5, 6])) {
            dayText = timeText ? "Daily" : "Always Active";
        } else if (arraysEqual(days, [1, 2, 3, 4, 5])) {
            dayText = "Weekdays";
        } else if (arraysEqual(days, [0, 6])) {
            dayText = "Weekends";
        } else {
            dayText = summarizeDays(days);
        }

        return {
            days: dayText,
            time: timeText || undefined,
        };
    }

    function summarizeDays(days: number[]): string {
        let shortDayNames = dayNames.map((n) => n.substring(0, 3));
        const ranges: string[] = [];

        let start = days[0];
        let previous = days[0];

        for (let i = 1; i <= days.length; i++) {
            const current = days[i];

            if (current === previous + 1) {
                previous = current;
                continue;
            }

            if (start === previous) {
                if (days.length === 1) {
                    ranges.push(dayNames[start]);
                } else {
                    ranges.push(shortDayNames[start]);
                }
            } else if (previous === start + 1) {
                // Two consecutive days look better separated by commas.
                ranges.push(shortDayNames[start]);
                ranges.push(shortDayNames[previous]);
            } else {
                ranges.push(
                    `${shortDayNames[start]} \u2013\n ${shortDayNames[previous]}`,
                );
            }

            start = current;
            previous = current;
        }

        return ranges.join(", ");
    }

    function arraysEqual(a: number[], b: number[]): boolean {
        return (
            a.length === b.length &&
            a.every((value, index) => value === b[index])
        );
    }
</script>

<PageHeader title="Policies">
    {#snippet subtitle()}
        Define how wards access their resources and request additional access.
    {/snippet}
</PageHeader>

<div class="toolbar">
    <a href="#/policies/new">
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
                {@const summary = scheduleSummary(policy.schedule)}

                <tr>
                    <td>
                        <strong>
                            {policy.name}
                        </strong>

                        <div class="status">
                            <StatusDot
                                label={policy.disabled ? "Disabled" : "Active"}
                                color={policy.disabled
                                    ? "var(--color-text-muted)"
                                    : "var(--color-success)"}
                                --font-size="0.85rem"
                            />
                        </div>
                    </td>

                    <td>{wards[policy.wardId]}</td>

                    <td>
                        <div class="schedule">
                            <strong>{summary.days}</strong>

                            {#if summary.time}
                                <div class="time">
                                    {summary.time}
                                </div>
                            {/if}
                        </div>
                    </td>

                    <td>
                        <AllowanceSummary
                            dailyTime={minutes(policy.access.dailyTimeMinutes)}
                            sessionLength={minutes(
                                policy.access.maxSessionMinutes,
                            )}
                            unlocks={unlocks(policy.access.dailyUnlocks)}
                        />
                    </td>

                    <td>
                        <span class:text-muted={!policy.override.allowed}>
                            {overrideLabel(policy.override.requirement) ||
                                "Disabled"}
                        </span>
                    </td>

                    <td>
                        {#if policy.override.allowance}
                            <AllowanceSummary
                                extension
                                dailyTime={minutes(
                                    policy.override.allowance.dailyTimeMinutes,
                                )}
                                sessionLength={minutes(
                                    policy.override.allowance.maxSessionMinutes,
                                )}
                                unlocks={unlocks(
                                    policy.override.allowance.dailyUnlocks,
                                )}
                            />
                        {/if}
                    </td>

                    <td>
                    <a href="#/policies/{policy.id}">
                        <button class="cta-button"> Edit </button>
                        </a>
                    </td>
                </tr>
            {/each}
        </tbody>
    </table>
</div>

<style>
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

    .schedule {
        display: flex;
        flex-direction: column;
        gap: 2px;
    }

    .schedule .time {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }
</style>
