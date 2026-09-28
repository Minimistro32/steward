<script lang="ts">
  import AppShell from "./lib/components/AppShell.svelte";
  import Router, { router, replace } from "svelte-spa-router";
  import Login from "./lib/pages/Login.svelte";

  import { onMount } from "svelte";
  import { currentUser } from "./lib/session";
  import { restoreSession } from "./lib/api/authApi";
  import { ApiError } from "./lib/api/client";
  let checking = $state(true);
  let sessionError = $state(false);
  async function checkSession() {
    try { await restoreSession(); sessionError = false; }
    catch (error) { sessionError = !(error instanceof ApiError && error.status === 401); }
    finally { checking = false; }
  }
  onMount(() => {
    void checkSession();
    const timer = setInterval(() => { void checkSession(); }, 60000);
    return () => clearInterval(timer);
  });
  $effect(() => {
    if (checking || sessionError) return;
    if (!$currentUser && router.location !== "/login") void replace("/login");
    else if ($currentUser?.type === "member" && router.location !== "/requests") void replace("/requests");
    else if ($currentUser && router.location === "/login") void replace("/");
  });

  // Components
  import Overview from "./lib/pages/Overview.svelte";
  import Agents from "./lib/pages/Agents.svelte";
  import UserForm from "./lib/pages/UserForm.svelte";
  import Users from "./lib/pages/Users.svelte";
  import Wards from "./lib/pages/Wards.svelte";
  import WardForm from "./lib/pages/WardForm.svelte";
  import Policies from "./lib/pages/Policies.svelte";
  import PolicyForm from "./lib/pages/PolicyForm.svelte";
  import Requests from "./lib/pages/Requests.svelte";

  const routes = {
    "/": Overview,
    "/agents": Agents,
    "/users": Users,
    "/users/new": UserForm,
    "/users/:id": UserForm,
    "/wards": Wards,
    "/wards/new": WardForm,
    "/wards/:id": WardForm,
    "/policies": Policies,
    "/policies/new": PolicyForm,
    "/policies/:id": PolicyForm,
    "/requests": Requests,
  };
</script>

{#if checking}
  <main class="session-status" role="status">Checking your session…</main>
{:else if sessionError}
  <main class="session-status" role="alert">Couldn’t connect to Steward. <button class="cta-button" onclick={checkSession}>Try again</button></main>
{:else if !$currentUser}
  <Login />
{:else}
  {#key $currentUser.id}
    <AppShell>
      {#if $currentUser.type === "member"}
        <Requests />
      {:else}
        <Router {routes} />
      {/if}
    </AppShell>
  {/key}
{/if}

<style>
  .session-status { min-height: 100svh; display: grid; place-content: center; gap: 1rem; }
</style>
