<div class="topbar">
    <div class="brand">
        <div class="brand-name">Steward</div>
        <div class="brand-tagline">Giving you room to grow.</div>
    </div>

    <div class="status">
        <span class="dot"></span>

        System Healthy
    </div>
</div>

<style>
    .topbar {
        height: 75px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 var(--space-7);
        background: var(--color-surface);
        border-bottom: 1px solid var(--color-border);
    }

    .brand {
        display: flex;
        flex-direction: column;
        line-height: 1;
    }

    .brand-name {
        font-size: 1.5rem;
        font-weight: 700;
        letter-spacing: -0.02em;
        color: var(--color-text);
        padding-bottom: var(--space-1);
    }

    .brand-tagline {
        margin-top: 4px;
        font-size: 0.9rem;
        font-weight: 500;
        color: var(--color-text-muted);
        letter-spacing: 0.01em;
    }

    .status {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        color: var(--color-text-muted);
    }

    .dot {
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: var(--color-success);
    }
</style>
