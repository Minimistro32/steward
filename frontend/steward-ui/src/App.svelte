<script lang="ts">
  import AppShell from "./lib/components/AppShell.svelte";
  import Router, { router } from "svelte-spa-router";
  import Login from "./lib/pages/Login.svelte";

  // Components
  import Overview from "./lib/pages/Overview.svelte";
  import Agents from "./lib/pages/Agents.svelte";
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
    "/wards": Wards,
    "/wards/new": WardForm,
    "/wards/:id": WardForm,
    "/policies": Policies,
    "/policies/new": PolicyForm,
    "/policies/:id": PolicyForm,
    "/requests": Requests,
  };
</script>

{#if router.location === "/login"}
  <Login />
{:else}
  <AppShell>
    <Router {routes} />
  </AppShell>
{/if}
