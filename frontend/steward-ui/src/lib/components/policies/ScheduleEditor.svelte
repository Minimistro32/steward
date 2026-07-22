<script>
    let selectedDays = ["Mon", "Tue", "Wed", "Thu", "Fri"];
    const days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
</script>

<h2>Schedule</h2>

<p class="text-muted">When should this policy be enforced?</p>

<div class="days">
    {#each days as day}
        <button class:selected={selectedDays.includes(day)}>
            {day}
        </button>
    {/each}
</div>

<div class="row">
    <label>
        Start Time
        <input type="time" value="20:00" />
    </label>

    <label>
        End Time
        <input type="time" value="07:00" />
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
