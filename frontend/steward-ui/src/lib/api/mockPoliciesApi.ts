import type { Policy } from "../models/policies/Policy";

export function getPolicies(): Policy[] {
    return [
        {
            id: "1",
            name: "Gaming Restrictions",
            tags: ["gaming"],
            disabled: false,
            wardId: "alice",
            createdAt: new Date(),
            modifiedAt: new Date(),
            schedule: {
                days: [1, 2, 3, 4, 5],
                startTime: "20:00",
                endTime: "07:00"
            },
            access: {
                dailyTimeMinutes: 60,
                maxSessionMinutes: 30,
                dailyUnlocks: 3
            },
            override: {
                allowed: true,
                requirement: "delay",
                allowance: {
                    dailyTimeMinutes: 30,
                    maxSessionMinutes: 15,
                    dailyUnlocks: 1
                }
            }
        },

        {
            id: "2",
            name: "School Night Rules",
            tags: ["school"],
            disabled: false,
            wardId: "kids",
            createdAt: new Date(),
            modifiedAt: new Date(),
            schedule: {
                days: [0, 1, 2, 4],
                startTime: "18:00",
                endTime: "07:00"
            },
            access: {
                dailyTimeMinutes: 90,
                maxSessionMinutes: 45,
                dailyUnlocks: 2
            },
            override: {
                allowed: false,
                allowance: {}
            }
        },

        {
            id: "3",
            name: "Weekend Relaxed",
            tags: [],
            disabled: true,
            wardId: "bob",
            createdAt: new Date(),
            modifiedAt: new Date(),
            schedule: {
                days: [6],
                startTime: "09:00",
                endTime: "23:00"
            },
            access: {
                maxSessionMinutes: 60,
                dailyUnlocks: 5
            },
            override: {
                allowed: true,
                requirement: "userApproval",
                allowance: {
                    dailyTimeMinutes: 60,
                    maxSessionMinutes: 30,
                    dailyUnlocks: 1
                }
            }
        }
    ];
}