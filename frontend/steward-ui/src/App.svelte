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

  /* Export the route definition object
  export default {
    // Exact path
    "/": Home,

    // Using named parameters, with last being optional
    "/hello/:first/:last?": Name,

    // Wildcard parameter
    // Included twice to match both `/wild` (and nothing after) and `/wild/*` (with anything after)
    "/wild": Wild,
    "/wild/*": Wild,

    // Catch-all, must be last
    "*": NotFound,
  };*/
</script>

<AppShell>
  <Router {routes} />
</AppShell>
