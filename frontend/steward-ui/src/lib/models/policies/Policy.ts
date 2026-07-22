import type { Allowance } from "./Allowance";
import type { OverridePolicy } from "./OverridePolicy";
import type { Schedule } from "./Schedule";

export interface Policy {
    id?: string;
    createdAt?: Date;
    modifiedAt?: Date;

    name: string;
    tags: string[];
    disabled: boolean;
    
    wardId: string;

    schedule: Schedule;

    access: Allowance;

    override: OverridePolicy;
}