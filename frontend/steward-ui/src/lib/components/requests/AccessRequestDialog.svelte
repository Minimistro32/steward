<script lang="ts">
    import type {
        AccessOption,
        AccessRequest,
        AccessResponse,
        OverrideAction,
    } from "../../models";
    import {
        postAccessRequest,
        postOverrideRequest,
        completeOverrideRequest,
    } from "../../api/accessApi";

    let {
        option,
        userId,
        onclose,
        oncomplete,
    }: {
        option: AccessOption;
        userId: number;
        onclose?: () => void;
        oncomplete?: () => void;
    } = $props();

    let dialogElement = $state<HTMLDialogElement>();
    let requestedMinutes = $state((() => option.maxRequestMinutes ?? 1)());
    let loading = $state(false);
    let response = $state<AccessResponse | null>(null);
    let error = $state<string | null>(null);

    let challengeInput = $state("");
    let challengeMismatch = $state(false);
    let submittedChallengeInput = $state<string | null>(null);

    let remainingSeconds = $state(0);
    let countdownInterval: ReturnType<typeof setInterval> | null = null;

    const isOverride = $derived(option.state === "overrideAvailable");

    $effect(() => {
        if (dialogElement && !dialogElement.open) {
            dialogElement.showModal();
        }
    });

    // $effect(() => {
    //     if (dialogElement && !dialogElement.open) {
    //         dialogElement.showModal();
    //     }

    //     const handleVisibilityChange = () => {
    //         if (document.hidden && dialogElement?.open) {
    //             close();
    //         }
    //     };

    //     const handleBlur = () => {
    //         if (dialogElement?.open) {
    //             close();
    //         }
    //     };

    //     document.addEventListener("visibilitychange", handleVisibilityChange);
    //     window.addEventListener("blur", handleBlur);

    //     return () => {
    //         document.removeEventListener(
    //             "visibilitychange",
    //             handleVisibilityChange,
    //         );
    //         window.removeEventListener("blur", handleBlur);
    //     };
    // });

    function submit() {
        if (loading || requestedMinutes <= 0) {
            return;
        }

        submitRequest();
    }

    async function submitRequest() {
        loading = true;
        error = null;

        try {
            let body: AccessRequest = {
                policyId: option.policyId,
                requestedMinutes: requestedMinutes,
            };

            response = isOverride
                ? await postOverrideRequest(userId, body)
                : await postAccessRequest(userId, body);

            if (response.requirement === "delay" && response.availableAt) {
                startDelayCountdown(response.availableAt);
            }
        } catch (e) {
            error = e instanceof Error ? e.message : "Something went wrong.";
        } finally {
            loading = false;
        }
    }

    async function completeOverride() {
        if (loading || !response?.overrideRequestId) {
            return;
        }

        if (response.requirement === "randomText") {
            submittedChallengeInput = challengeInput;

            if (challengeInput !== response.challengeText) {
                challengeMismatch = true;
                return;
            }
        }

        loading = true;
        error = null;

        try {
            const action: OverrideAction = {
                userId,
                ...(response.requirement === "randomText"
                    ? { challengeText: challengeInput }
                    : {}),
            };

            response = await completeOverrideRequest(
                response.overrideRequestId,
                action,
            );
        } catch (e) {
            error = e instanceof Error ? e.message : "Something went wrong.";
        } finally {
            loading = false;
        }
    }

    function startDelayCountdown(availableAt: string) {
        stopDelayCountdown();

        const update = () => {
            const remaining = Math.max(
                0,
                new Date(availableAt).getTime() - Date.now(),
            );

            remainingSeconds = Math.floor(remaining / 1000);

            if (remaining <= 0) {
                stopDelayCountdown();
            }
        };

        update();

        countdownInterval = setInterval(update, 250);
    }

    function stopDelayCountdown() {
        if (countdownInterval !== null) {
            clearInterval(countdownInterval);
            countdownInterval = null;
        }
    }

    function formatCountdown(totalSeconds: number): string {
        const hours = Math.floor(totalSeconds / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = totalSeconds % 60;

        if (hours > 0) {
            return `${hours}:${String(minutes).padStart(2, "0")}:${String(
                seconds,
            ).padStart(2, "0")}`;
        }

        return `${minutes}:${String(seconds).padStart(2, "0")}`;
    }

    function close() {
        response = null;
        error = null;
        challengeInput = "";
        submittedChallengeInput = null;
        challengeMismatch = false;
        requestedMinutes = 1;
        onclose?.();
    }

    function requirementText(): string {
        if (!response?.requirement) {
            return "";
        }

        switch (response.requirement) {
            case "delay":
                return remainingSeconds > 0
                    ? "You need to wait before access can be granted."
                    : "The wait is over. Hit continue to grant the override.";

            case "randomText":
                return "Type the following text exactly to continue.";

            case "userApproval":
                return "Another user must approve this request.";
        }
    }
</script>

<dialog bind:this={dialogElement} class="dialog">
    <div class="dialog-content">
        <header>
            <div>
                <h2>
                    {isOverride ? "Request Override" : "Request Access"}
                </h2>

                <p>
                    {option.devices.map((d) => d.name).join(", ")}
                </p>
            </div>

            <button
                type="button"
                class="close-button"
                onclick={close}
                aria-label="Close"
            >
                &#215;
            </button>
        </header>

        {#if response === null}
            <div class="request-form">
                <label for="requested-minutes"> How many minutes? </label>

                <input
                    type="range"
                    min="1"
                    max={option.maxRequestMinutes ?? 120}
                    bind:value={requestedMinutes}
                />

                <input
                    id="requested-minutes"
                    type="number"
                    min="1"
                    max={option.maxRequestMinutes ?? undefined}
                    bind:value={requestedMinutes}
                />

                {#if option.maxRequestMinutes !== null}
                    <p class="hint">
                        Maximum: {option.maxRequestMinutes} minutes
                    </p>
                {/if}

                {#if error}
                    <p class="error">{error}</p>
                {/if}

                <button
                    type="button"
                    class="cta-button"
                    onclick={submit}
                    disabled={loading || requestedMinutes <= 0}
                >
                    {#if loading}
                        Submitting...
                    {:else}
                        Submit
                    {/if}
                </button>
            </div>
        {:else}
            <div class="response">
                {#if response.state === "granted"}
                    <h3>Access Granted</h3>

                    <p>
                        You have been granted
                        {requestedMinutes} minutes of access.
                    </p>

                    <button
                        type="button"
                        class="cta-button"
                        onclick={() => {
                            oncomplete?.();
                            close();
                        }}
                    >
                        Done
                    </button>
                {:else if response.state === "pending"}
                    <h3>Override Pending</h3>

                    {#if response.requirement}
                        <p>{requirementText()}</p>

                        {#if response.requirement === "delay" && response.availableAt}
                            <div class="delay-section">
                                {#if remainingSeconds > 0}
                                    <p class="delay-countdown">
                                        {formatCountdown(remainingSeconds)}
                                    </p>

                                    <p>
                                        You can submit at {new Date(
                                            response.availableAt,
                                        ).toLocaleTimeString([], {
                                            hour: "numeric",
                                            minute: "2-digit",
                                        })}
                                    </p>
                                {:else}
                                    <button
                                        type="button"
                                        class="cta-button"
                                        onclick={completeOverride}
                                        disabled={loading}
                                    >
                                        {loading ? "Checking..." : "Continue"}
                                    </button>
                                {/if}
                            </div>
                        {:else if response.requirement === "randomText"}
                            <div class="challenge-section">
                                <p
                                    class:error-challenge={challengeMismatch}
                                    class="challenge"
                                    aria-label="Challenge text"
                                    oncopy={(event) => event.preventDefault()}
                                    oncut={(event) => event.preventDefault()}
                                    oncontextmenu={(event) =>
                                        event.preventDefault()}
                                >
                                    {#each response.challengeText as character, index}
                                        <span
                                            class:mismatch={challengeMismatch &&
                                                submittedChallengeInput !==
                                                    null &&
                                                submittedChallengeInput[
                                                    index
                                                ] !== character}
                                        >
                                            {character === " "
                                                ? "\u00A0"
                                                : character}
                                        </span>
                                    {/each}
                                </p>

                                <textarea
                                    class="challenge-input"
                                    bind:value={challengeInput}
                                    autocomplete="off"
                                    autocapitalize="off"
                                    spellcheck="false"
                                    placeholder="Type the text above"
                                    onpaste={(event) => event.preventDefault()}
                                    ondrop={(event) => event.preventDefault()}
                                    rows="3"
                                    onkeydown={(event) => {
                                        if (
                                            event.key === "Enter" &&
                                            !event.shiftKey
                                        ) {
                                            event.preventDefault();
                                            completeOverride();
                                        }
                                    }}
                                ></textarea>

                                {#if error}
                                    <p class="error">{error}</p>
                                {/if}

                                <button
                                    type="button"
                                    class="cta-button"
                                    onclick={completeOverride}
                                    disabled={loading || !challengeInput.trim()}
                                >
                                    {#if loading}
                                        Checking...
                                    {:else}
                                        Submit
                                    {/if}
                                </button>
                            </div>
                        {/if}
                    {:else}
                        <p>Your override has been requested.</p>
                    {/if}

                    <button type="button" class="cta-button" onclick={close}>
                        Close
                    </button>
                {:else if response.state === "overrideRequired"}
                    <h3>Override Required</h3>

                    <p>
                        Normal access is not available for this request. You can
                        request an override instead.
                    </p>

                    <button
                        type="button"
                        class="cta-button"
                        onclick={() => {
                            response = null;
                        }}
                    >
                        Request Override
                    </button>
                {:else}
                    <h3>Access Unavailable</h3>

                    <p>This request cannot currently be granted.</p>

                    <button type="button" class="cta-button" onclick={close}>
                        Close
                    </button>
                {/if}
            </div>
        {/if}
    </div>
</dialog>

<style>
    dialog {
        position: fixed;
        inset: 0;
        margin: auto;

        width: min(550px, calc(100vw - 2rem));
        max-height: calc(100vh - 2rem);

        border: none;
        padding: 0;

        background: transparent;
        overflow: visible;
    }

    dialog::backdrop {
        background: rgba(11, 15, 9, 0.4);
        backdrop-filter: blur(2px);
    }

    .dialog-content {
        display: flex;
        flex-direction: column;

        max-height: calc(100vh - 2rem);
        box-sizing: border-box;

        padding: var(--space-7);
        overflow-y: auto;

        background: var(--color-surface);
        border: 1px solid var(--color-border);
        border-radius: var(--radius-lg);
        box-shadow: var(--shadow-lg);
    }

    header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;

        gap: var(--space-6);
        margin-bottom: var(--space-6);
    }

    header h2 {
        margin: 0;
        font-size: 1.5rem;
        line-height: 1.2;
    }

    header p {
        margin: var(--space-2) 0 0;
        color: var(--color-text-muted);
        font-size: 0.95rem;
    }

    .close-button {
        flex-shrink: 0;

        width: 2rem;
        height: 2rem;
        padding: 0;

        display: grid;
        place-items: center;

        border: none;
        border-radius: var(--radius-md);
        background: transparent;

        color: var(--color-text-muted);
        font-size: 1.5rem;
        line-height: 1;

        cursor: pointer;
    }

    .close-button:hover {
        background: var(--color-surface-raised);
        color: var(--color-text);
    }

    .request-form,
    .response {
        display: flex;
        flex-direction: column;
    }

    .response {
        text-align: center;
    }

    .request-form button {
        margin-top: var(--space-2);
    }

    .response button {
        margin-top: var(--space-5);
    }

    label {
        font-weight: 600;
    }

    input {
        width: 100%;
        box-sizing: border-box;
    }

    .hint {
        margin-top: var(--space-1);
        color: var(--color-text-muted);
        font-size: 0.85rem;
    }

    .error {
        margin: 0;

        color: var(--color-danger);
        font-size: 0.9rem;
    }

    .response h3 {
        margin: 0;
        font-size: 1.35rem;
    }

    .response p {
        margin: 0;
        line-height: 1.5;
        color: var(--color-text-muted);
    }

    .challenge {
        box-sizing: border-box;

        width: 100%;
        max-height: 10rem;
        overflow-y: auto;
        overflow-x: hidden;

        margin: 0;
        padding: var(--space-4) var(--space-6);

        text-align: left;

        border: 1px solid var(--color-border);
        border-radius: var(--radius-md);
        background: var(--color-surface-raised);

        font-family: monospace;
        font-size: 0.95rem;
        line-height: 1.5;

        white-space: normal;
        overflow-wrap: anywhere;

        user-select: none;
        -webkit-user-select: none;
        cursor: default;
    }

    .challenge .mismatch {
        color: var(--color-danger);
        text-decoration: underline;
        text-decoration-thickness: 2px;
        text-underline-offset: 2px;
    }

    .challenge-section {
        display: flex;
        flex-direction: column;
        gap: var(--space-3);

        margin-top: var(--space-2);
    }

    .challenge-input {
        box-sizing: border-box;

        width: 100%;
        max-height: 15rem;

        padding: var(--space-3) var(--space-6);

        font-family: monospace;
        font-size: 0.95rem;
        line-height: 1.5;

        overflow-y: auto;
        overflow-x: hidden;

        resize: vertical;
        white-space: pre-wrap;
        overflow-wrap: anywhere;
    }

    .delay-countdown {
        margin: var(--space-4) 0 var(--space-8);

        font-family: monospace;
        font-size: 2rem;
        font-weight: 600;
        line-height: 1.2;

        color: var(--color-text);
        text-align: center;
        letter-spacing: 0.05em;
    }

    .delay-section {
        display: flex;
        flex-direction: column;
        align-items: stretch;
        gap: var(--space-3);
        text-align: center;
    }

    .delay-section p {
        margin: 0;
        color: var(--color-text-muted);
    }

    @media (max-width: 600px) {
        dialog {
            width: calc(100vw - 1rem);
            max-height: calc(100vh - 1rem);
        }

        .dialog-content {
            max-height: calc(100vh - 1rem);
            padding: var(--space-5);
        }
    }
</style>
