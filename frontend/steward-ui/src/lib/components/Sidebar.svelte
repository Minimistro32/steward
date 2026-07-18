<script>
    import { link } from "svelte-spa-router";
    import active from "svelte-spa-router/active";
</script>

<nav class="sidebar">
    <img
        class="logo"
        src="/src/assets/steward-logo.svg"
        alt="Green steward logo with budding leaf"
    />

    <div class="links">
        <a use:link use:active href="/" class="active"> Overview </a>
        
        <a use:link use:active href="/agents"> Agents </a>
        <!-- linked from agent health: `features.md/1+2`-->
        
        <a use:link use:active href="/wards"> Wards </a>
        <!--`features.md/3`-->
        
        <a use:link use:active href="/policies"> Policies </a>
        <!--`features.md/4`-->
        
        <a use:link use:active href="/requests"> Requests </a>
        <!--`features.md/5`-->
    </div>

    <div class="logo-credit">
        <a
            href="https://www.vecteezy.com/free-vector/s-logo"
            target="_blank"
            rel="noopener noreferrer"
        >
            Logo design by Vecteezy
        </a>
    </div>
</nav>

<style>
    .sidebar {
        background: var(--color-surface);
        border-right: 1px solid var(--color-border);
        padding: var(--space-2) var(--space-5);

        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .logo {
        width: 90px;
        padding-bottom: var(--space-5);
    }

    .links {
        display: flex;
        flex-direction: column;
        gap: var(--space-2);
        width: 100%;
    }

    .links a {
        color: var(--color-text-muted);
        text-decoration: none;
        padding: var(--space-3);
        border-radius: var(--radius-sm);
        text-align: center;
    }

    a:hover {
        background: var(--color-surface-raised);
        color: var(--color-text);
    }

    a.active {
        height: 50%;
        position: relative;
        color: var(--color-brand-light);
    }

    a.active::before {
        content: "";
        position: absolute;
        left: 0;
        top: 25%;
        height: 50%;
        width: 3px;
        background: var(--color-brand-light);
        border-radius: 999px;
    }

    .logo-credit {
        margin-top: auto;
        padding-top: var(--space-6);

        font-size: 0.65rem;
        color: var(--color-text-muted);
        opacity: 0.6;
        text-align: center;
    }

    .logo-credit a {
        color: inherit;
        text-decoration: none;
    }

    .logo-credit a:hover {
        color: var(--color-text);
        text-decoration: underline;
    }
</style>
