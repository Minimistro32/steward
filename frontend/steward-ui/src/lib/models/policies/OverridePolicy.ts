import type { Allowance } from "..";

// TODO: Remove override policy completely. Allowed will completely depend on whether an OverrideAllowance is provided.
// Move optional requirement and allowance (which should also become optional and renamed to OverrideAccess) to the policy directly.
// A null `OverrideRequirement` means that nothing is required to submit an override request.

export interface OverridePolicy {
    allowed: boolean;
    requirement?: OverrideRequirement;
    allowance: Allowance;
}

export type OverrideRequirement =
    | "delay"
    | "randomText"
    | "userApproval";