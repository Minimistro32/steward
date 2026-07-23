<script lang="ts">
    import { onMount } from "svelte";

    import PageHeader from "../components/ui/PageHeader.svelte";
    import ScheduleEditor from "../components/policies/ScheduleEditor.svelte";
    import Card from "../components/ui/Card.svelte";
    import Checkbox from "../components/ui/Checkbox.svelte";

    import type { Policy } from "../models/policies/Policy";
    import { createDefaultPolicy } from "../models/policies/Policy";
    import type { OverrideRequirement } from "../models/policies/OverridePolicy";

    import { getPolicies } from "../api/mockPoliciesApi";

    export let params;
    let policy: Policy;
    let notFound = false;
    let loading = true;
    
    onMount(() => {
        if (params?.id) {
            // const found = await policyApi.get(id);
            const found = getPolicies().find((p) => p.id === params.id);

            if (!found) {
                notFound = true;
                return;
            }

            policy = found;
        } else {
            policy = createDefaultPolicy();
        }

        loading = false;
    });

    function savePolicy() {
        if (!validatePolicy()) {
            return;
        }

        console.log("Saving policy:", policy);

        // Later:
        // await policyApi.save(policy);
    }

    let errors: string[] = [];
    function validatePolicy(): boolean {
        errors = [];

        if (!policy.name.trim()) {
            errors.push("Policy name is required.");
        }

        if (!policy.wardId) {
            errors.push("A ward must be selected.");
        }

        if (
            policy.access.maxSessionMinutes &&
            policy.access.dailyTimeMinutes &&
            policy.access.maxSessionMinutes > policy.access.dailyTimeMinutes
        ) {
            errors.push(
                "Maximum session length cannot exceed daily allowance.",
            );
        }

        if (policy.override.allowed) {
            if (
                policy.override.allowance.maxSessionMinutes &&
                policy.override.allowance.dailyTimeMinutes &&
                policy.override.allowance.maxSessionMinutes >
                    policy.override.allowance.dailyTimeMinutes
            ) {
                errors.push(
                    "Override session length cannot exceed override allowance.",
                );
            }
        }

        return errors.length === 0;
    }

    function setRequirement(requirement: OverrideRequirement) {
        if (policy.override.requirement === requirement) {
            policy.override.requirement = undefined;
        } else {
            policy.override.requirement = requirement;
        }
    }
</script>

{#if loading}

    <p>Loading...</p>

{:else if notFound}
    <PageHeader title="Policy Not Found">
        {#snippet subtitle()}
            The policy you requested does not exist.
        {/snippet}
    </PageHeader>

    <a href="#/policies">
        <button class="cta-button">
            Return to Policies
        </button>
    </a>

{:else if policy}
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
            <ScheduleEditor bind:schedule={policy.schedule} />
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
                        <Checkbox
                            label="Delay"
                            checked={policy.override.requirement === "delay"}
                            onchange={() => setRequirement("delay")}
                        />

                        <Checkbox
                            label="Type random text"
                            checked={policy.override.requirement ===
                                "randomText"}
                            onchange={() => setRequirement("randomText")}
                        />

                        <Checkbox
                            label="Another user approval"
                            checked={policy.override.requirement ===
                                "userApproval"}
                            onchange={() => setRequirement("userApproval")}
                        />
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

        {#if errors.length > 0}
            <div class="errors">
                {#each errors as error}
                    <p>{error}</p>
                {/each}
            </div>
        {/if}

        <div class="actions">
            <a href="#/policies">
                <button class="cta-button"> Cancel </button>
            </a>

            <button onclick={savePolicy} class="primary"> Save Policy </button>
        </div>
    </div>
</div>
{/if}

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

    .errors {
        background: rgba(229, 83, 83, 0.1);
        border: 1px solid var(--color-failure);
        border-radius: var(--radius-md);

        padding: var(--space-4);

        margin-bottom: var(--space-4);
    }

    .errors p {
        color: var(--color-failure);
        margin: 0;
    }
</style>
