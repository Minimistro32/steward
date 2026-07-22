<script lang="ts">
    import PageHeader from "../components/ui/PageHeader.svelte";
    import ScheduleEditor from "../components/policies/ScheduleEditor.svelte";
    import Card from "../components/ui/Card.svelte";
    import Checkbox from "../components/ui/Checkbox.svelte";

    let allowOverrides = true;
</script>

<div class="editor">
    <PageHeader title="Create Policy" --margin-bottom="0px">
        {#snippet subtitle()}
            Define how a <a href="#/wards">ward</a> is managed.
        {/snippet}
    </PageHeader>

    <Card>
        <h2>General</h2>

        <label>
            Policy Name
            <input placeholder="Example: Gaming Restrictions" />
        </label>

        <label>
            Ward
            <select>
                <option>Alice</option>
                <option>Family Gaming</option>
                <option>School Devices</option>
            </select>
        </label>

        <label>
            Tags
            <input placeholder="gaming, school, weekday" />
        </label>
    </Card>

    <Card>
        <ScheduleEditor />
    </Card>

    <Card>
        <h2>Access Allowance</h2>

        <p class="text-muted">
            Normal access limits before an override is requested.
        </p>

        <div class="allowance-grid">
            <label>
                Daily Time
                <input value="60" />
                <span>minutes</span>
            </label>

            <label>
                Maximum Session
                <input value="30" />
                <span>minutes</span>
            </label>

            <label>
                Daily Unlocks
                <input value="3" />
                <span>per day</span>
            </label>
        </div>
    </Card>

    <Card>
        <div class="override">
            <h2>Override Requests</h2>

            <Checkbox
                label="Allow override requests"
                bind:checked={allowOverrides}
            />

            {#if allowOverrides}
                <h3>Requirements</h3>
                <div class="nested">
                    <Checkbox label="Delay" />

                    <Checkbox label="Type random phrase" />

                    <Checkbox label="Another user approval" />
                </div>

                <h3>Override Allowance</h3>

                <div class="allowance-grid">
                    <label>
                        Additional Time
                        <input value="30" />
                        <span>minutes</span>
                    </label>

                    <label>
                        Maximum Request Length
                        <input value="15" />
                        <span>minutes</span>
                    </label>

                    <label>
                        Additional Unlocks
                        <input value="1" />
                        <span>unlock</span>
                    </label>
                </div>
            {/if}
        </div>
    </Card>

    <div class="actions">
        <button class="cta-button"> Cancel </button>

        <button class="primary"> Save Policy </button>
    </div>
</div>

<style>
    .editor {
        display: flex;
        flex-direction: column;
        max-width: 50vw;
        margin: 0 auto;

        gap: var(--space-4);
    }

    label {
        display: flex;
        flex-direction: column;

        gap: var(--space-2);

        margin-bottom: var(--space-4);

        color: var(--color-text-muted);

        font-size: 0.9rem;
    }

    .allowance-grid {
        display: grid;

        grid-template-columns: repeat(3, minmax(0, 1fr));

        gap: var(--space-4);
    }

    .allowance-grid label {
        background: var(--color-surface-raised);

        padding: var(--space-4);

        border-radius: var(--radius-md);
    }

    .allowance-grid span {
        font-size: 0.8rem;

        color: var(--color-text-muted);
    }

    .override,
    h3 {
        padding-top: var(--space-3);
    }

    .nested {
        margin-left: var(--space-5);

        padding-left: var(--space-5);

        border-left: 2px solid var(--color-border);
    }

    .actions {
        display: flex;

        justify-content: flex-end;

        gap: var(--space-3);
        grid-column: span 2;
    }

    .primary {
        background: var(--color-brand);

        color: white;

        border: none;

        padding: var(--space-2) var(--space-4);

        border-radius: var(--radius-sm);

        cursor: pointer;
    }

    a {
        color: var(--color-text-muted);
        text-decoration: none;
        font-weight: bold;
    }

    a:hover {
        color: var(--color-brand-light);
    }
</style>
