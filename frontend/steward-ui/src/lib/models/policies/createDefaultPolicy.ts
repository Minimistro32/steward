import type { Policy } from "./Policy";

export function createDefaultPolicy(): Policy {
    return {
        name: "",

        wardId: "",

        tags: [],

        disabled: false,

        schedule: {
            enabled: true,
            days: [],
            startTime: "",
            endTime: ""
        },

        access: {},

        override: {
            allowed: false,

            requireDelay: false,
            // escalatingDelay: false,

            requireRandomPhrase: false,

            requireUserApproval: false,

            allowance: {}
        }
    };
}