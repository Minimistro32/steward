import type { Device, Resource, OverrideRequirement } from "..";

export interface AccessOption {
    policyId: number;
    requirement: OverrideRequirement | null;

    grantedResources: Resource[];
    devices: Device[];

    state: AccessState;

    maxRequestMinutes: number | null;

    scheduleEndsAt: string | null;

    effectiveMinutesRemaining: number | null;

    dailyMinutesRemaining: number | null;

    unlocksRemaining: number | null;

    unlockedUntil: string | null;
}

export type AccessState =
    | "available"
    | "overrideAvailable"
    | "overridePending"
    | "unlocked"
    | "unavailable";