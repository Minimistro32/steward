<script>
    let { request } = $props();
</script>

<div class="item">
    <div class={`dot ${request.status}`}></div>

    <div class="content">
        <div class="resource">
            {request.resource}
        </div>

        <div class="meta">
            {request.person} · {request.id}
            · {request.age}
        </div>

        <p>
            "{request.reason}"
        </p>
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

    .dot.approved {
        background: var(--color-success);
    }

    .dot.denied {
        background: var(--color-failure);
    }

    .resource {
        font-weight: 600;
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
</style>
