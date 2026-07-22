<script lang="ts">
    import PageHeader from "../components/ui/PageHeader.svelte";
    import ScheduleEditor from "../components/policies/ScheduleEditor.svelte";
    import Card from "../components/ui/Card.svelte";
    import Checkbox from "../components/ui/Checkbox.svelte";

    import type { Policy } from "../models/policies/Policy";
    import { createDefaultPolicy } from "../models/policies/createDefaultPolicy";
    
    export let params;
    const id = params?.id;
    
    export let policy: Policy;

    if (id) {
        // policy = await policyApi.get(id);
        policy = {
            id: id,
            name: `Test getting id from route ${id}`,
            tags: [],
            disabled: false,
            wardId: "test",
            schedule: {
                enabled: true,
                days: [],
                startTime: "",
                endTime: "",
            },
            access: {},
            override: {
                allowed: true,
                requireDelay: false,
                requireRandomPhrase: false,
                requireUserApproval: false,
                allowance: {},
            },
        };
    } else {
        policy = createDefaultPolicy();
    }
</script>

<div class="centered">
    <PageHeader title="Create Policy" --margin-bottom="var(--space-4)">
        {#snippet subtitle()}
            Define how a <a href="#/wards">ward</a> is managed.
        {/snippet}
    </PageHeader>

    <div class="editor">
        <Card>
            <h2>General</h2>

            <label>
                Policy Name
                <input
                    bind:value={policy.name}
                    placeholder="Enter a policy name"
                />
            </label>

            <label>
                Ward
                <select
                    bind:value={policy.wardId}
                    class:placeholder={!policy.wardId}
                >
                    <option value="" disabled hidden>Select a ward</option>
                    <option>Alice</option>
                    <option>Family Gaming</option>
                    <option>School Devices</option>
                </select>
            </label>

            <label>
                Tags
                <input
                    bind:value={policy.tags}
                    placeholder="gaming, school, weekday"
                />
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
                    <input
                        type="number"
                        placeholder="∞"
                        bind:value={policy.access.dailyTimeMinutes}
                    />
                    <span>minutes</span>
                </label>

                <label>
                    Maximum Session
                    <input
                        type="number"
                        placeholder="∞"
                        bind:value={policy.access.maxSessionMinutes}
                    />
                    <span>minutes</span>
                </label>

                <label>
                    Daily Unlocks
                    <input
                        type="number"
                        placeholder="∞"
                        bind:value={policy.access.dailyUnlocks}
                    />
                    <span>per day</span>
                </label>
            </div>
        </Card>

        <Card>
            <div class="override">
                <h2>Override Requests</h2>

                <Checkbox
                    label="Allow override requests"
                    bind:checked={policy.override.allowed}
                />

                {#if policy.override.allowed}
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
                            <input
                                type="number"
                                placeholder="∞"
                                bind:value={
                                    policy.override.allowance.dailyTimeMinutes
                                }
                            />
                            <span>minutes</span>
                        </label>

                        <label>
                            Maximum Request Length
                            <input
                                type="number"
                                placeholder="∞"
                                bind:value={
                                    policy.override.allowance.maxSessionMinutes
                                }
                            />
                            <span>minutes</span>
                        </label>

                        <label>
                            Additional Unlocks
                            <input
                                type="number"
                                placeholder="∞"
                                bind:value={
                                    policy.override.allowance.dailyUnlocks
                                }
                            />
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
</div>

<style>
    .centered {
        margin: 0 auto;
        max-width: 50vw;
    }

    .editor {
        display: flex;
        flex-direction: column;
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

    select.placeholder {
        color: var(--color-text-muted);
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
