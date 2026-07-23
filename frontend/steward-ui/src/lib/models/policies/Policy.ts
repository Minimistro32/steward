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

export function createDefaultPolicy(): Policy {
    return {
        name: "",

        wardId: "",

        tags: [],

        disabled: false,

        schedule: {
            days: [],
            startTime: "",
            endTime: ""
        },

        access: {},

        override: {
            allowed: false,

            requireDelay: false,
            // escalatingDelay: false,

            requireRandomText: false,

            requireUserApproval: false,

            allowance: {}
        }
    };
}