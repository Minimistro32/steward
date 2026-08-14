import type { OverrideRequirement } from "..";

export interface AccessResponse {
    state: AccessRequestStatus;

    overrideRequestId?: number;

    requirement: OverrideRequirement | null;

    availableAt: string | null;

    challengeText: string | null;
}

export enum AccessRequestStatus {
    Granted = "granted",
    Invalid = "invalid",
    OverrideRequired = "overrideRequired",
    Pending = "pending",
    Unavailable = "unavailable",
}