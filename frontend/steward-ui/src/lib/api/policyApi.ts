import { type Policy } from "../models/policies";

const mockPolicies: Policy[] = [
    {
        id: "1",
        name: "Gaming Restrictions",
        tags: ["gaming"],
        disabled: false,
        wardId: "alice",
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString(),
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
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString(),
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
        createdAt: new Date().toISOString(),
        modifiedAt: new Date().toISOString(),
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


export async function getPolicies(): Promise<Policy[]> {
    return structuredClone(mockPolicies);
}

export async function getPolicy(id: string): Promise<Policy | undefined> {
    return structuredClone(
        mockPolicies.find((p) => p.id === id)
    );
}

export async function createPolicy(
    policy: Omit<Policy, "id" | "createdAt" | "modifiedAt">
): Promise<Policy> {

    const now = new Date();

    const createdPolicy: Policy = {
        ...structuredClone(policy),

        id: crypto.randomUUID(),

        createdAt: now.toISOString(),
        modifiedAt: now.toISOString(),
    };

    mockPolicies.push(createdPolicy);

    return structuredClone(createdPolicy);
}

export async function updatePolicy(policy: Policy): Promise<void> {
    const index = mockPolicies.findIndex((p) => p.id === policy.id);

    if (index >= 0) {
        mockPolicies[index] = structuredClone(policy);
    }
}

export async function deletePolicy(id: string): Promise<void> {
    const index = mockPolicies.findIndex((p) => p.id === id);

    if (index >= 0) {
        mockPolicies.splice(index, 1);
    }
}