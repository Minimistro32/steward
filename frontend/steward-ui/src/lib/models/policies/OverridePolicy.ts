import type { Allowance } from "./Allowance";

export interface OverridePolicy {
    allowed: boolean;

    requireDelay: boolean;

    // escalatingDelay: boolean;

    requireRandomPhrase: boolean;

    requireUserApproval: boolean;

    allowance: Allowance;
}