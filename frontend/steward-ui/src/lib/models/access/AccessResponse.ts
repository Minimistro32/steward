import type { OverrideRequirement } from "..";

export interface AccessResponse {
    state: AccessRequestStatus;

    overrideRequestId?: number;

    requirement: OverrideRequirement | null;

    availableAt: string | null;

    challengeText: string | null;
}

export type AccessRequestStatus =
    | "granted"
    | "alreadyUnlocked"
    | "invalid"
    | "overrideRequired"
    | "pending"
    | "unavailable";