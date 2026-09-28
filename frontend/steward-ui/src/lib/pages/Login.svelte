<script lang="ts">
    import { onMount } from "svelte";
    import { getLoginUsers, signIn } from "../api/authApi";
    import { ApiError } from "../api/client";


    let users = $state<{ id: number; name: string }[]>([]);
    let userId = $state("");
    let pin = $state("");
    let showPin = $state(false);
    let loading = $state(true);
    let loadError = $state(false);
    let message = $state("");
    let submitting = $state(false);

    async function loadUsers() {
        loading = true;
        loadError = false;
        try {
            users = await getLoginUsers();
        } catch {
            loadError = true;
        } finally {
            loading = false;
        }
    }

    onMount(() => { void loadUsers(); });

    function enterDigit(digit: number) {
        if (pin.length < 128) pin += String(digit);
        message = "";
    }

    async function submit(event: SubmitEvent) {
        event.preventDefault();
        if (submitting) return;
        submitting = true;
        message = "";
        try { await signIn(Number(userId), pin); }
        catch (error) {
            message = error instanceof ApiError && error.status === 401
                ? "That PIN isn’t correct. Try again."
                : error instanceof ApiError && error.status === 429
                  ? "Too many attempts. Please wait a minute and try again."
                  : "Couldn’t sign in. Check your connection and try again.";
        } finally { pin = ""; submitting = false; }
    }

</script>

<svelte:head>
    <title>Sign in · Steward</title>
</svelte:head>

<main class="login-page">
    <div class="login-layout">
        <section class="login-card" aria-labelledby="login-title">
            <header>
                <img src="/steward-logo.svg" alt="" width="110" height="110" />
                <!-- <p class="brand">Steward</p> -->
                <h1 id="login-title">Welcome to Steward</h1>
                <p class="subtitle">Choose your name and enter your PIN.</p>
            </header>

            <form onsubmit={submit}>
                <div class="field">
                    <label for="login-user">Your name</label>
                    <select id="login-user" bind:value={userId} required disabled={loading || loadError || users.length === 0}
                        onchange={() => { pin = ""; message = ""; }}>
                        <option value="" disabled>{loading ? "Loading names…" : "Choose your name"}</option>
                        {#each users as user (user.id)}
                            <option value={String(user.id)}>{user.name}</option>
                        {/each}
                    </select>
                    {#if loadError}
                        <p class="field-help" role="alert">Couldn’t load names. <button class="text-button" type="button" onclick={loadUsers}>Try again</button></p>
                    {:else if !loading && users.length === 0}
                        <p class="field-help" role="status">No accounts are available. Ask your administrator to set up your account.</p>
                    {/if}
                </div>

                <div class="field">
                    <label for="login-pin">Your PIN</label>
                    <div class="pin-input">
                        <input id="login-pin" type={showPin ? "text" : "password"} inputmode="numeric"
                            pattern="[0-9]+" autocomplete="current-password"
                            placeholder="Enter your PIN" maxlength="128" value={pin}
                            oninput={(event) => { pin = event.currentTarget.value.replace(/[^0-9]/g, ""); message = ""; }} />
                        <button class="visibility" type="button" aria-label={showPin ? "Hide PIN" : "Show PIN"}
                            aria-pressed={showPin} onclick={() => showPin = !showPin}>
                            {#if showPin}Hide{:else}Show{/if}
                        </button>
                    </div>
                    <p class="field-help">No PIN set? Leave it empty.</p>
                </div>

                <div class="keypad" role="group" aria-label="PIN keypad">
                    {#each [1, 2, 3, 4, 5, 6, 7, 8, 9] as digit}
                        <button type="button" onclick={() => enterDigit(digit)}>{digit}</button>
                    {/each}
                    <button class="key-action" type="button" disabled={!pin} onclick={() => { pin = ""; message = ""; }}>Clear</button>
                    <button type="button" onclick={() => enterDigit(0)}>0</button>
                    <button class="key-action" type="button" aria-label="Delete last digit" disabled={!pin} onclick={() => { pin = pin.slice(0, -1); message = ""; }}>
                        <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" aria-hidden="true">
                            <path d="M9 5h12v14H9l-7-7 7-7Z" stroke-linejoin="round" />
                            <path d="m12 9 6 6m0-6-6 6" stroke-linecap="round" />
                        </svg>
                    </button>
                </div>

                <button class="sign-in" type="submit" disabled={!userId || submitting || loading || loadError}>{submitting ? "Signing in…" : "Sign in"} <span aria-hidden="true">→</span></button>
                {#if message}<p class="message" role="status">{message}</p>{/if}
            </form>

            <details>
                <summary>Forgot your PIN?</summary>
                <p>Ask your Steward administrator to help you reset your PIN.</p>
            </details>
        </section>
        <footer>Giving you room to grow.</footer>
    </div>
</main>

<style>
    .login-page {
        min-height: 100svh;
        display: grid;
        place-items: center;
        padding: var(--space-7) var(--space-4);
        background: radial-gradient(ellipse at 50% 20%, var(--color-brand-muted), transparent 65%), var(--color-background);
    }
    .login-layout { width: 100%; max-width: 420px; }
    .login-card {
        padding: var(--space-7);
        background: var(--color-surface);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-lg);
        box-shadow: var(--shadow-md);
    }
    header { text-align: center; margin-bottom: var(--space-6); }
    header img { object-fit: contain; }
    h1 { margin: 0 0 var(--space-2); font-size: 1.5rem; font-weight: 600; letter-spacing: -0.03em; }
    .subtitle, .field-help, .message, details p { color: var(--color-text-muted); font-size: 0.875rem; line-height: 1.6; }
    .subtitle { margin: 0; }
    form, .field { display: grid; gap: var(--space-2); }
    form { gap: var(--space-5); }
    label { font-size: 0.8rem; font-weight: 500; }
    select, input { width: 100%; min-width: 0; min-height: 48px; font: inherit; font-size: 0.9rem; }
    .pin-input { position: relative; }
    .pin-input input { padding-right: 4.5rem; }
    button { cursor: pointer; }
    button:disabled { cursor: default; opacity: 0.45; }
    .visibility { position: absolute; right: 4px; top: 4px; height: 40px; padding: 0 var(--space-3); border: 0; border-radius: var(--radius-sm); background: transparent; color: var(--color-text-muted); font-size: 0.8rem; }
    .keypad { display: grid; grid-template-columns: repeat(3, 1fr); gap: var(--space-2); }
    .keypad button { display: grid; place-items: center; min-height: 52px; border: 1px solid var(--color-border); border-radius: var(--radius-sm); background: var(--color-surface-button); color: var(--color-text); font-size: 1.25rem; touch-action: manipulation; }
    .keypad .key-action { font-size: 0.8rem; color: var(--color-text-muted); }
    .keypad button:hover:not(:disabled), .visibility:hover { background: var(--color-surface-raised); }
    .keypad button:active:not(:disabled) { background: var(--color-brand-muted); border-color: var(--color-brand); }
    .sign-in { display: flex; justify-content: center; align-items: center; gap: var(--space-3); min-height: 48px; border: 1px solid var(--color-brand); border-radius: var(--radius-sm); color: var(--color-text); background: var(--color-brand); font-weight: 600; }
    .sign-in:hover:not(:disabled) { background: var(--color-brand-hover); }
    button:focus-visible, summary:focus-visible, input:focus-visible, select:focus-visible { outline: 2px solid var(--color-brand-light); outline-offset: 3px; }
    .field-help, .message { margin: 0; }
    .text-button { color: var(--color-brand-light); background: none; border: 0; padding: 0; text-decoration: underline; }
    details { margin-top: var(--space-6); border-top: 1px solid var(--color-border); padding-top: var(--space-5); }
    summary { cursor: pointer; color: var(--color-text-muted); font-size: 0.85rem; }
    details p { margin-bottom: 0; }
    footer { text-align: center; color: var(--color-text-muted); font-size: 0.75rem; padding-top: var(--space-5); }
    @media (max-width: 480px) {
        .login-page { padding: var(--space-5) var(--space-3); }
        .login-card { padding: var(--space-6); }
        .keypad button { min-height: 56px; }
    }
</style>
