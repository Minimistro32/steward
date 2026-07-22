import type { Allowance } from "./Allowance";
import type { OverridePolicy } from "./OverridePolicy";
import type { Schedule } from "./Schedule";

export interface Policy {
    id: string;

    wardId: string;

    name: string;

    tags: string[];

    isActive: boolean;

    createdAt: Date;
    modifiedAt: Date;

    schedule: Schedule;

    access: Allowance;

    overrides: OverridePolicy;
}