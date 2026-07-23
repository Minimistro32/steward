<script lang="ts">
    import Card from "../ui/Card.svelte";
    import type { Policy } from "../../models/policies/Policy";
    import { formatTimeRange } from "../../models/policies/Schedule";

    import { getPolicies } from "../../api/mockPoliciesApi";

    let policies: Policy[] = getPolicies();

    const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    function getDayRanges(days: number[]) {
        if (!days.length) return [];

        const ranges = [];

        let start = days[0];
        let previous = days[0];

        for (let i = 1; i < days.length; i++) {
            const current = days[i];

            if (current === previous + 1) {
                previous = current;
            } else {
                ranges.push({
                    start: start + 1,
                    end: previous + 1,
                });

                start = current;
                previous = current;
            }
        }

        ranges.push({
            start: start + 1,
            end: previous + 1,
        });

        return ranges;
    }
</script>

<Card title="Policy Timeline">
    <div class="timeline">
        <div class="days">
            <div class="header-spacer"></div>
            {#each days as day}
                <span>{day}</span>
            {/each}
        </div>

        {#each policies as policy, policyIndex}
            <div class="time-label row" style="top:{48 + policyIndex * 42}px;">
                {formatTimeRange(policy.schedule)}
            </div>
            <span
                class="row divider"
                style="
                    left:{((1 / 8) * 100) - 0.1}%;
                    top:{28 + policyIndex * 42}px;
                "
            ></span>
            {#each getDayRanges(policy.schedule.days) as range}
                <div
                    class="event row"
                    style="
                        left:{((range.start / 8) * 100) + 0.5}%;
                        width:{(((range.end - range.start + 1) / 8) * 100) - 0.5}%;
                        top:{48 + policyIndex * 42}px;
                    "
                >
                    {policy.name}
                </div>
            {/each}
        {/each}
    </div>
</Card>

<style>
    .timeline {
        position: relative;
        height: 180px;
    }

    .header-spacer {
        border-right: 1px solid var(--color-border);
        border-bottom: 1px solid var(--color-border);
    }

    .days {
        display: grid;
        grid-template-columns: repeat(8, 1fr);
        margin-bottom: var(--space-4);
        color: var(--color-text-muted);
        font-size: 0.8rem;
        text-align: center;
    }

    .days span {
        padding-bottom: var(--space-2);
        border-bottom: 1px solid var(--color-border);
    }

    .row {
        position: absolute;

        height: 34px;
        line-height: 34px;
    }

    .divider {
        height: 50px;
        border-left: 1px solid var(--color-border);
    }

    .time-label {
        color: var(--color-text-muted);

        width: 11%;
        font-size: 0.85rem;
        text-align: center;
    }

    .event {
        padding: 0 var(--space-3);

        background: rgba(134, 188, 73, 0.12);
        border: 1px solid rgba(134, 188, 73, 0.25);

        color: var(--color-brand-light);

        border-radius: var(--radius-sm);

        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }
</style>
