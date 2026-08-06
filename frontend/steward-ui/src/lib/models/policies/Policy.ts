import type { Allowance, OverridePolicy, Schedule } from "../";

export interface Policy {
    id?: number;
    createdAt?: string;
    modifiedAt?: string;

    name: string;
    tags: string[];
    disabled: boolean;
    
    wardId?: number;

    schedule: Schedule;

    access: Allowance;

    override: OverridePolicy;
}

export function createDefaultPolicy(): Policy {
    return {
        name: "",
        
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
            allowance: {}
        }
    };
}