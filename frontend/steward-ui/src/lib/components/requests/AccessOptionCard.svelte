<script lang="ts">
    import type { AccessOption, AccessState } from "../../models";
    import Card from "../ui/Card.svelte";
    import StatusDot from "../ui/StatusDot.svelte";

    let {
        option,
        onclick,
    }: {
        option: AccessOption;
        onclick?: () => void;
    } = $props();

    function minutes(value: number | null): string {
        return value == null ? "∞" : `${value} min`;
    }

    function unlocks(value: number | null): string {
        return value == null ? "∞" : value.toString();
    }

    function scheduleEnd(value: string | null): string {
        if (!value) {
            return "No schedule limit";
        }

        return new Date(value).toLocaleTimeString([], {
            hour: "numeric",
            minute: "2-digit",
        });
    }

    const status = $derived.by(() => {
        switch (option.state) {
            case "available":
                return {
                    label: "Available",
                    color: "var(--color-success)",
                    button: "Request Access",
                    disabled: false,
                };

            case "overrideAvailable":
                return {
                    label: "Override Available",
                    color: "var(--color-warning)",
                    button: "Request Override",
                    disabled: false,
                };

            case "unavailable":
                return {
                    label: "Unavailable",
                    color: "var(--color-danger)",
                    button: "Daily Limits Reached",
                    disabled: true,
                };
        }
    });
</script>

<div class="access-option-card">
    <Card>
        <div class="header-grid">
            <h2>{option.devices.map((d) => d.name).join(", ")}</h2>

            <div class="primary-stat">
                <strong>
                    {minutes(option.effectiveMinutesRemaining)}
                </strong>

                <StatusDot
                    label={status.label}
                    color={status.color}
                    --font-size="0.85rem"
                    --justified="right"
                />
                <!-- <span> Remaining </span> -->
            </div>
        </div>

        <div class="resources">
            {#each option.grantedResources as resource}
                <span class="tag">
                    {resource.name}
                </span>
            {/each}
        </div>

        <div class="details">
            <div class="stat">
                <span>Daily Remaining</span>

                <strong>
                    {minutes(option.dailyMinutesRemaining)}
                </strong>
            </div>

            <div class="stat">
                <span>Session Length</span>

                <strong>
                    {minutes(option.maxRequestMinutes)}
                </strong>
            </div>

            <div class="stat">
                <span>Unlocks</span>

                <strong>
                    {unlocks(option.unlocksRemaining)}
                </strong>
            </div>
        </div>

        {#if option.scheduleEndsAt}
            <div class="schedule">
                Schedule ends at
                {scheduleEnd(option.scheduleEndsAt)}
            </div>
        {/if}

        <button class="cta-button" {onclick} disabled={status.disabled}>
            {status.button}
        </button>
    </Card>
</div>

<style>
    .access-option-card {
        min-width: 405px;
        max-width: 550px;
    }

    .header-grid {
        display: grid;
        grid-template-columns: 2fr 1fr;
    }

    h2 {
        font-size: 1.45rem;
    }

    .resources {
        display: flex;
        flex-wrap: wrap;
        gap: var(--space-2);

        margin-bottom: var(--space-6);
    }

    .tag {
        background: var(--color-surface-raised);
        border-radius: var(--radius-md);

        padding: var(--space-1) var(--space-3);

        font-size: 0.85rem;
    }

    .primary-stat {
        display: flex;
        flex-direction: column;
        gap: var(--space-1);

        text-align: right;
        justify-items: right;
    }

    .primary-stat strong {
        font-size: 2.25rem;
        line-height: 1;
    }

    .details {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: var(--space-4);

        margin-bottom: var(--space-5);
    }

    .stat {
        display: flex;
        flex-direction: column;
        gap: var(--space-1);
    }

    .stat span {
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .schedule {
        color: var(--color-text-muted);
        font-size: 0.9rem;

        padding-top: var(--space-1);
        margin-bottom: var(--space-2);
        text-align: center;
    }

    button {
        width: 100%;
    }
</style>
