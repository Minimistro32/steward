<script lang="ts">
    import { currentUser } from "../session";
    import { signOut as endSession } from "../api/authApi";
    let logoutError = $state(false);
    let accountMenu: HTMLDetailsElement;
    let accountTrigger: HTMLElement;

    function closeMenu() {
        if (accountMenu) accountMenu.open = false;
    }

    function handleOutsideClick(event: MouseEvent) {
        if (event.target instanceof Node && !accountMenu?.contains(event.target)) {
            closeMenu();
        }
    }

    function handleKeydown(event: KeyboardEvent) {
        if (event.key === "Escape" && accountMenu?.open) {
            closeMenu();
            accountTrigger?.focus();
        }
    }

    async function signOut() {
        logoutError = false;
        try { await endSession(); closeMenu(); }
        catch { logoutError = true; }
    }

</script>

<svelte:window onclick={handleOutsideClick} onkeydown={handleKeydown} />

<div class="topbar">
    <div class="brand-with-logo">
        {#if $currentUser?.type === "member"}<img src="/steward-logo.svg" alt="" width="44" height="48" />{/if}
        <div class="brand">
            <div class="brand-name">Steward</div>
            <div class="brand-tagline">Giving you room to grow.</div>
        </div>
    </div>

    <details class="account-menu" bind:this={accountMenu}>
        <summary class="account-trigger" bind:this={accountTrigger}>
            <span class="account-identity">
                <span class="account-name">{$currentUser?.name}</span>
                {#if $currentUser?.type === "admin"}
                    <span class="account-role">Admin</span>
                {/if}
            </span>
            <span class="chevron" aria-hidden="true"></span>
        </summary>

        <div class="account-dropdown">
            <button type="button" class="sign-out" onclick={signOut}>
                <svg
                    width="18"
                    height="18"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="1.75"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    aria-hidden="true"
                >
                    <path d="M9 5H5a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h4" />
                    <path d="M9 12h12m-4-4 4 4-4 4" />
                </svg>
                Log out
            </button>
            {#if logoutError}<p role="alert">Couldn’t log out. Try again.</p>{/if}
        </div>
    </details>
</div>

<style>
    .brand-with-logo { display: flex; align-items: center; gap: var(--space-3); }
    .brand-with-logo img { object-fit: contain; }
    .topbar {
        height: 75px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 var(--space-7);
        gap: var(--space-4);
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
    .account-menu {
        position: relative;
        flex-shrink: 0;
    }

    .account-trigger {
        display: flex;
        align-items: center;
        gap: var(--space-3);
        min-height: 44px;
        padding: var(--space-2) var(--space-3);
        border: none;
        border-radius: var(--radius-md);
        color: var(--color-text);
        cursor: pointer;
        list-style: none;
    }

    .account-trigger::-webkit-details-marker {
        display: none;
    }

    .account-trigger:hover,
    .account-menu[open] .account-trigger {
        background: var(--color-surface-raised);
    }

    .account-trigger:focus-visible,
    .sign-out:focus-visible {
        outline: 2px solid var(--color-brand-light);
        outline-offset: 3px;
    }

    .account-name {
        max-width: 10rem;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        font-weight: 600;
        font-size: 1.25rem;
    }

    .chevron {
        width: 6px;
        height: 6px;
        border-right: 2px solid var(--color-text-muted);
        border-bottom: 2px solid var(--color-text-muted);
        transform: rotate(45deg);
    }

    .account-menu[open] .chevron {
        transform: rotate(225deg);
    }

    .account-dropdown {
        position: absolute;
        right: 0;
        top: calc(100% + var(--space-2));
        z-index: 100;
        width: min(160px, calc(100vw - 2rem));
        padding: var(--space-2);
        background: var(--color-surface);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-md);
        box-shadow: var(--shadow-lg);
    }

    .account-identity {
        display: flex;
        flex-direction: column;
        align-items: flex-end;
        gap: 2px;
        line-height: 1.25;
    }

    .account-role {
        color: var(--color-text-muted);
        font-size: 0.65rem;
        font-weight: 400;
    }

    .sign-out {
        display: flex;
        align-items: center;
        gap: var(--space-3);
        width: 100%;
        min-height: 44px;
        padding: var(--space-3);
        border: none;
        border-radius: var(--radius-md);
        background: transparent;
        color: var(--color-text);
        font: inherit;
        text-align: left;
        cursor: pointer;
    }

    .sign-out:hover {
        background: var(--color-surface-raised);
    }

    @media (max-width: 600px) {
        .topbar {
            padding: 0 var(--space-4);
        }

        .brand-tagline {
            font-size: 0.75rem;
        }

        .account-name {
            max-width: 6rem;
        }
    }
</style>
