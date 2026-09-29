<script lang="ts">
    import type { RequestActivity } from "../../models";

    let {
        request,
        currentUserId,
        canManage = false,
        onapprove,
        onreject,
    }: {
        request: RequestActivity;
        currentUserId?: number;
        canManage?: boolean;
        onapprove?: (request: RequestActivity) => void;
        onreject?: (request: RequestActivity) => void;
    } = $props();

    let canRespond = $derived(
        canManage && currentUserId !== undefined &&
        request.status === "pending" &&
            request.requirement === "userApproval" &&
            request.userId !== currentUserId,
    );

    let resourceLabel = $derived(
        request.resources.length
            ? request.resources.join(", ")
            : "Unknown resource",
    );

    let requestType = $derived(
        request.requirement === "userApproval"
            ? "Approval"
            : request.requirement === "randomText"
              ? "Text"
              : request.requirement === "delay"
                ? "Delay"
                : "Override",
    );

    export function formatRelativeTime(value: string | Date): string {
        const date = new Date(value);
        const now = new Date();

        const diffMs = now.getTime() - date.getTime();
        const diffMinutes = Math.floor(diffMs / 60_000);
        const diffHours = Math.floor(diffMs / 3_600_000);

        if (diffMinutes < 1) {
            return "Just now";
        }

        if (diffMinutes < 60) {
            return `${diffMinutes} ${diffMinutes === 1 ? "minute" : "minutes"} ago`;
        }

        if (diffHours < 24) {
            return `${diffHours} ${diffHours === 1 ? "hour" : "hours"} ago`;
        }

        const yesterday = new Date(now);
        yesterday.setDate(now.getDate() - 1);

        if (date.toDateString() === yesterday.toDateString()) {
            return "Yesterday";
        }

        return date.toLocaleDateString(undefined, {
            month: "short",
            day: "numeric",
            year:
                date.getFullYear() !== now.getFullYear()
                    ? "numeric"
                    : undefined,
        });
    }
</script>

<div class="item">
    <div class={`dot ${request.status}`}></div>

    <div>
        <div class="heading">
            <div class="resource">
                {resourceLabel}
            </div>

            <div class="duration">—</div>

            <div class="duration">
                {request.requestedMinutes} min
            </div>
        </div>

        <div class="meta">
            {request.userName} · {requestType}
            · {formatRelativeTime(request.createdAt)}
        </div>

        {#if request.reason}
            <p class="reason">{request.reason}</p>
        {/if}

        {#if canRespond}
            <div class="actions">
                <button
                    class="action approve"
                    title="Approve request"
                    aria-label="Approve request"
                    onclick={() => onapprove?.(request)}
                >
                    ✓
                </button>

                <button
                    class="action reject"
                    title="Reject request"
                    aria-label="Reject request"
                    onclick={() => onreject?.(request)}
                >
                    &times;
                </button>
            </div>
        {/if}
    </div>
</div>

<style>
    .item {
        position: relative;

        display: flex;
        gap: var(--space-4);

        margin-bottom: var(--space-6);
    }

    .dot {
        position: absolute;

        left: calc(var(--space-7) * -1 + 2px);
        top: 5px;

        width: 10px;
        height: 10px;

        border-radius: 50%;
    }

    .dot.pending {
        background: var(--color-warning);
    }

    .dot.granted {
        background: var(--color-success);
    }

    .dot.rejected {
        background: var(--color-danger);
    }

    .heading {
        display: flex;
        align-items: center;
        gap: var(--space-3);
    }

    .resource {
        min-width: 0;
        font-weight: 600;
    }

    .duration {
        flex-shrink: 0;

        color: var(--color-text-muted);
        font-size: 0.9rem;
        font-weight: 100;
    }

    .meta {
        color: var(--color-text-muted);
        font-size: 0.75rem;
        margin-top: 2px;
    }

    p {
        margin: var(--space-2) 0 0;

        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .reason { white-space: pre-wrap; overflow-wrap: anywhere; }

    .actions {
        display: flex;
        gap: var(--space-4);
        margin-top: var(--space-3);
        margin-left: var(--space-6);
        /* justify-content: center; */
    }

    .action {
        display: inline-flex;
        align-items: center;
        justify-content: center;

        width: 38px;
        height: 28px;

        border: 1px solid currentColor;
        border-radius: var(--radius-md);
        background: transparent;

        font-size: 1rem;
        font-weight: 700;

        cursor: pointer;
    }

    .action.approve {
        color: var(--color-brand-light);
    }

    .action.reject {
        color: var(--color-danger);
    }

    .action:hover {
        background: color-mix(in srgb, currentColor 10%, transparent);
    }
</style>
