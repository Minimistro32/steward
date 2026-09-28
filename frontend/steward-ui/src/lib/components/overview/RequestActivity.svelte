<script lang="ts">
    import { currentUser } from "../../session";
    import { link } from "svelte-spa-router";
    import { onMount } from "svelte";

    import Card from "../ui/Card.svelte";
    import RequestTimelineElement from "./RequestTimelineElement.svelte";

    import {
        approveOverrideRequest,
        rejectOverrideRequest,
        getRequestActivity,
    } from "../../api";
    import type { RequestActivity as RequestActivityModel } from "../../models";

    let {
        showMakeRequest = true,
    }: {
        showMakeRequest?: boolean;
    } = $props();

    let requests = $state<RequestActivityModel[]>([]);

    let currentUserId = $derived($currentUser?.id);

    let pendingRequests = $derived(
        requests.filter((request) => request.status === "pending"),
    );

    let historyRequests = $derived(
        requests.filter((request) => request.status !== "pending"),
    );

    async function loadRequests() {
        requests = await getRequestActivity();
    }

    async function approveRequest(request: RequestActivityModel) {
        if (!$currentUser || $currentUser.type !== "admin") return;
        await approveOverrideRequest(request.id, {
            userId: $currentUser.id,
        });

        await loadRequests();
    }

    async function rejectRequest(request: RequestActivityModel) {
        if (!$currentUser || $currentUser.type !== "admin") return;
        await rejectOverrideRequest(request.id, {
            userId: $currentUser.id,
        });

        await loadRequests();
    }

    onMount(loadRequests);
</script>

<Card title="Request Activity">
    {#snippet actions()}
        {#if showMakeRequest}
            <a use:link href="/requests">
                <button class="cta-button">Make Request</button>
            </a>
        {/if}
    {/snippet}

    {#if pendingRequests.length || historyRequests.length}
        <div class="scrolling-container">
            <div class="timeline">
                {#if pendingRequests.length}
                    <section class="pending">
                        <h4>Pending</h4>

                        {#each pendingRequests as request}
                            <RequestTimelineElement
                                {request}
                                {currentUserId}
                                canManage={$currentUser?.type === "admin"}
                                onapprove={approveRequest}
                                onreject={rejectRequest}
                            />
                        {/each}
                    </section>
                {/if}

                {#if historyRequests.length}
                    <section
                        class:divider={pendingRequests.length}
                        class="history"
                    >
                        <h4>History</h4>

                        {#each historyRequests as request}
                            <RequestTimelineElement {request} />
                        {/each}
                    </section>
                {/if}
            </div>
        </div>
    {:else}
        <div class="empty-state">
            <div class="empty-icon">✓</div>
            <h4>No request activity</h4>
            <p>New access requests will appear here.</p>
        </div>
    {/if}
</Card>

<style>
    h4 {
        margin: 0 0 var(--space-3);
        font-size: 0.85rem;
        color: var(--color-text-muted);
        text-transform: uppercase;
        letter-spacing: 0.05em;
    }

    .divider {
        margin-top: var(--space-6);
        padding-top: var(--space-6);
        border-top: 1px solid var(--color-border);
    }

    .scrolling-container {
        position: relative;
        max-height: var(--request-activity-height, 60vh);
        overflow-y: auto;

        padding-left: var(--space-4);
        padding-right: var(--space-2);
    }

    .timeline {
        position: relative;
        padding-left: var(--space-7);
    }

    .timeline::before {
        content: "";

        position: absolute;
        left: 7px;
        top: 0;
        bottom: 0;

        width: 1px;
        background: var(--color-border);
    }

    .empty-state {
        height: var(--request-activity-height, 60vh);

        min-height: var(--request-activity-empty-min-height, 220px);

        display: flex;
        flex-direction: column;

        align-items: center;
        justify-content: center;

        text-align: center;

        color: var(--color-text-muted);
    }

    .empty-icon {
        width: 36px;
        height: 36px;

        display: flex;
        align-items: center;
        justify-content: center;

        margin-bottom: var(--space-3);

        border-radius: 50%;

        background: rgba(134, 188, 73, 0.12);
        color: var(--color-success);

        font-weight: 700;
    }
</style>
