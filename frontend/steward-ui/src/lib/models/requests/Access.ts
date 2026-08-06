import type { Device, Resource } from "..";

export interface AccessResponse {
    options: AccessOption[];
}

export interface AccessOption {
    grantedResources: Resource[];
    devices: Device[];

    requiresOverride: boolean;

    maxRequestMinutes: number | null;

    scheduleEndsAt: string | null;

    effectiveMinutesRemaining: number | null;

    dailyMinutesRemaining: number | null;

    unlocksRemaining: number | null;
}