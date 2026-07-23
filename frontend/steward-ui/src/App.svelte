<script lang="ts">
  import AppShell from "./lib/components/AppShell.svelte";
  import Router from "svelte-spa-router";

  // Components
  import Overview from "./lib/pages/Overview.svelte";
  import Agents from "./lib/pages/Agents.svelte";
  // import Wards from "./lib/pages/Wards.svelte";
  import Policies from "./lib/pages/Policies.svelte";
  import PolicyForm from "./lib/pages/PolicyForm.svelte";
  // import Requests from "./lib/pages/Requests.svelte";

  const routes = {
    "/": Overview,
    "/agents": Agents,
    // "/wards": Wards,
    "/policies": Policies,
    "/policies/new": PolicyForm,
    "/policies/:id": PolicyForm,
    // "/requests": Requests,
  };
</script>

<AppShell>
  <Router {routes} />
</AppShell>
