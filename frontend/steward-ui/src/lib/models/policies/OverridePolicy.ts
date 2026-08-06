import type { Allowance } from "..";

export interface OverridePolicy {
    allowed: boolean;
    requirement?: OverrideRequirement;
    allowance: Allowance;
}

export type OverrideRequirement =
    | "delay"
    | "randomText"
    | "userApproval";