<script lang="ts">
    import { DayOfWeek, type Schedule } from "../../models/policies/Schedule";

    export let schedule: Schedule;

    const days = [
        { value: DayOfWeek.Monday, label: "Mon" },
        { value: DayOfWeek.Tuesday, label: "Tue" },
        { value: DayOfWeek.Wednesday, label: "Wed" },
        { value: DayOfWeek.Thursday, label: "Thu" },
        { value: DayOfWeek.Friday, label: "Fri" },
        { value: DayOfWeek.Saturday, label: "Sat" },
        { value: DayOfWeek.Sunday, label: "Sun" },
    ];

    function toggleDay(day: number) {
        if (schedule.days.includes(day)) {
            schedule.days = schedule.days.filter((d) => d !== day);
        } else {
            schedule.days = [...schedule.days, day];
        }
    }
</script>

<h2>Schedule</h2>

<p class="text-muted">When should this policy be enforced?</p>

<div class="days">
    {#each days as day}
        <button
            type="button"
            class:selected={schedule.days.includes(day.value)}
            onclick={() => toggleDay(day.value)}
        >
            {day.label}
        </button>
    {/each}
</div>

<div class="row">
    <label>
        Start Time
        <input type="time" bind:value={schedule.startTime} />
    </label>

    <label>
        End Time
        <input type="time" bind:value={schedule.endTime} />
    </label>
</div>

<style>
    .row {
        display: grid;

        grid-template-columns: 1fr 1fr;

        gap: var(--space-5);
    }

    .days {
        display: flex;

        gap: var(--space-2);

        margin-bottom: var(--space-5);
    }

    .days button {
        border: 1px solid var(--color-border);

        background: var(--color-surface-raised);

        color: var(--color-text-muted);

        padding: var(--space-2) var(--space-3);

        border-radius: 999px;

        cursor: pointer;
    }

    .days button.selected {
        background: var(--color-brand-muted);

        color: var(--color-brand-light);

        border-color: var(--color-brand);
    }
</style>
