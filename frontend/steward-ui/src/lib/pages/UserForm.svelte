<script lang="ts">
    import { onMount } from "svelte";
    import { push } from "svelte-spa-router";
    import PageHeader from "../components/ui/PageHeader.svelte";
    import Card from "../components/ui/Card.svelte";
    import { getUser, createUser, updateUser } from "../api/userApi";
    import { ApiError } from "../api/client";
    import { restoreSession } from "../api/authApi";
    import { currentUser, setSession } from "../session";

    let { params }: { params?: { id?: string } } = $props();
    const isNew = $derived(!params?.id);
    let name = $state("");
    let email = $state("");
    let type = $state<"admin" | "member">("member");
    let originalType = $state<"admin" | "member">("member");
    let hasPin = $state(false);
    let pin = $state("");
    let confirmation = $state("");
    let clearPin = $state(false);
    let loading = $state(true);
    let loadError = $state("");
    let errors = $state<string[]>([]);
    let saving = $state(false);

    async function load() {
        loading = true;
        loadError = "";
        try {
            if (params?.id) {
                const user = await getUser(params.id);
                if (!user) { loadError = "User not found."; return; }
                if (user.type === "admin" && user.id !== $currentUser?.id) {
                    loadError = "You can only edit your own account or member accounts.";
                    return;
                }
                if (typeof user.hasPin !== "boolean") {
                    loadError = "The server is running an older user API. Restart the Steward server before editing PINs.";
                    return;
                }
                name = user.name;
                email = user.email ?? "";
                type = originalType = user.type;
                hasPin = user.hasPin;
            }
        } catch (error) {
            loadError = error instanceof ApiError && error.status === 403 ? "You can only edit your own account or member accounts."
                : error instanceof ApiError && error.status === 404 ? "User not found." : "Couldn’t load this user. Try again.";
        } finally { loading = false; }
    }
    onMount(() => { void load(); });

    async function save(event: SubmitEvent) {
        event.preventDefault();
        if (saving) return;
        // Read the actual input values at submission, including browser autofill.
        // Do this before disabling the fieldset, which removes inputs from FormData.
        const fields = new FormData(event.currentTarget as HTMLFormElement);
        pin = String(fields.get("pin") ?? "");
        confirmation = String(fields.get("pinConfirmation") ?? "");
        errors = [];
        if (!name.trim()) errors.push("Name is required.");
        if (type === "admin" && !email.trim()) errors.push("Admins require an email address.");
        else if (email.trim() && (event.currentTarget as HTMLFormElement).querySelector<HTMLInputElement>("#user-email")?.validity.typeMismatch)
            errors.push("Enter a valid email address.");
        if (type === "admin" && !hasPin && !pin) errors.push("Admins require a PIN.");
        if (pin && !/^[0-9]{4,128}$/.test(pin)) errors.push("PIN must contain at least four digits (maximum 128).");
        if (pin !== confirmation) errors.push("PINs do not match.");
        if (errors.length) return;
        saving = true;
        try {
            const body = { name: name.trim(), email: email.trim() || null, type, pin: pin || undefined, clearPin: type === "member" && clearPin };
            const user = isNew ? await createUser(body) : await updateUser(Number(params!.id), body);
            if (typeof user.hasPin !== "boolean" || (pin && !user.hasPin) || (body.clearPin && user.hasPin)) {
                errors = ["The server did not confirm the PIN change. Restart the Steward server and try again."];
                return;
            }
            if (user.id === $currentUser?.id) {
                if (pin || clearPin || type !== originalType) setSession(null);
                else await restoreSession();
            }
            pin = confirmation = "";
            await push($currentUser ? "/users" : "/login");
        } catch (error) {
            errors = error instanceof ApiError && error.status === 403 ? ["You can only edit your own account or member accounts."]
                : error instanceof ApiError && error.errors.length ? error.errors : ["Couldn’t save this user. Please try again."];
        } finally { saving = false; }
    }
</script>

<div class="centered">
    <PageHeader title={isNew ? "Create User" : "Edit User"}>
        {#snippet subtitle()}Manage the account details and how this person signs in.{/snippet}
    </PageHeader>
    {#if loading}
        <p role="status">Loading user…</p>
    {:else if loadError}
        <p role="alert">{loadError}</p>
        <button class="cta-button" onclick={load}>Try again</button>
        <a href="#/users">Back to users</a>
    {:else}
        <form onsubmit={save} novalidate>
            <fieldset disabled={saving}>
                <Card>
                    <h2>Account</h2>
                    <label for="user-name">Name</label>
                    <input id="user-name" bind:value={name} autocomplete="off" required />
                    <label for="user-type">Account type</label>
                    <select id="user-type" bind:value={type} onchange={() => { if (type === "admin") clearPin = false; }}>
                        <option value="member">Member</option>
                        <option value="admin">Admin</option>
                    </select>
                    <p class="hint">{type === "admin" ? "Admins manage users, settings, and request approvals." : "Members request access. Their accounts are managed by admins."}</p>
                    <label for="user-email">Email {type === "member" ? "(optional)" : ""}</label>
                    <input id="user-email" type="email" bind:value={email} autocomplete="off" required={type === "admin"} />
                </Card>
                <Card>
                    <h2>Sign-in PIN</h2>
                    <p class="hint">{isNew ? (type === "admin" ? "Set a PIN with at least four digits for this admin." : "A PIN is optional. Without one, this member can sign in with an empty PIN.") : (hasPin ? "A PIN is set. Leave the fields empty to keep it unchanged." : "No PIN is currently set.")}</p>
                    {#if !isNew && hasPin && type === "member"}
                        <label class="clear"><input type="checkbox" bind:checked={clearPin} onchange={() => { pin = confirmation = ""; }} />Clear this member’s PIN</label>
                    {/if}
                    <label for="user-pin">{isNew ? "PIN" : "New PIN"}</label>
                    <input id="user-pin" name="pin" type="password" inputmode="numeric" autocomplete="new-password" maxlength="128" bind:value={pin} disabled={clearPin} required={type === "admin" && !hasPin} />
                    <label for="user-pin-confirm">Confirm PIN</label>
                    <input id="user-pin-confirm" name="pinConfirmation" type="password" inputmode="numeric" autocomplete="new-password" maxlength="128" bind:value={confirmation} disabled={clearPin} required={!!pin} />
                    {#if !isNew && Number(params?.id) === $currentUser?.id}<p class="hint">Changing your PIN or account type will require you to sign in again.</p>{/if}
                </Card>
                {#if errors.length}<div class="errors" role="alert">{#each errors as error}<p>{error}</p>{/each}</div>{/if}
                <div class="actions">
                    <button class="cta-button" type="button" onclick={() => push("/users")}>Cancel</button>
                    <button class="primary" type="submit">{saving ? "Saving…" : isNew ? "Create User" : "Save User"}</button>
                </div>
            </fieldset>
        </form>
    {/if}
</div>

<style>
    .centered { max-width: 700px; margin: 0 auto; }
    fieldset { display: grid; gap: var(--space-4); border: 0; margin: 0; padding: 0; min-width: 0; }
    h2 { margin-top: 0; }
    label { display: block; margin: var(--space-4) 0 var(--space-2); color: var(--color-text-muted); font-size: 0.9rem; }
    input:not([type="checkbox"]), select { width: 100%; min-height: 44px; }
    .hint { color: var(--color-text-muted); font-size: 0.85rem; line-height: 1.5; }
    .clear { display: flex; align-items: center; gap: var(--space-2); }
    .actions { display: flex; justify-content: flex-end; gap: var(--space-3); font-weight: bold; }
    a { color: var(--color-text-muted); text-decoration: none; }
    .primary { background: var(--color-brand); color: white; border: 0; padding: var(--space-2) var(--space-4); border-radius: var(--radius-sm); cursor: pointer; }
    .primary:disabled { opacity: 0.6; }
    .errors {
        background: rgba(229, 83, 83, 0.1);
        border: 1px solid var(--color-danger);
        border-radius: var(--radius-md);
        padding: var(--space-4);
        margin-bottom: var(--space-4);
    }
    .errors p { color: var(--color-danger); margin: 0; }
</style>
