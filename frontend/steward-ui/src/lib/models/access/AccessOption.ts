import type { Device, Resource } from "..";

export interface AccessOption {
    policyId: number;

    grantedResources: Resource[];
    devices: Device[];

    state: AccessState;

    maxRequestMinutes: number | null;

    scheduleEndsAt: string | null;

    effectiveMinutesRemaining: number | null;

    dailyMinutesRemaining: number | null;

    unlocksRemaining: number | null;
}

export type AccessState =
    | "available"
    | "overrideAvailable"
    | "unavailable";