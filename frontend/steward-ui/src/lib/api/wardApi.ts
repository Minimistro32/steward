import type { User, Ward } from "../models/wards";

const mockUsers: User[] = [
    {
        id: "alice",
        name: "Alice",
        agentSelections: {
            coldTurkey: {
                deviceIds: [
                    "gaming-pc",
                    "laptop",
                ],
            },
        },
    },

    {
        id: "bob",
        name: "Bob",
        agentSelections: {
            pihole: {
                deviceIds: [
                    "desktop",
                ],
            },

            esp32: {
                deviceIds: [
                    "switch",
                ],
            },
        },
    },

    {
        id: "charlie",
        name: "Charlie",
        agentSelections: {},
    },
];


const mockWards: Ward[] = [
    {
        id: "alice",
        name: "Alice",
        tags: [
            "gaming",
            "school",
        ],

        userIds: [
            "alice",
        ],

        agentSelections: {
            coldTurkey: {
                deviceIds: [
                    "gaming-pc",
                    "laptop",
                ],
                resourceIds: [
                    "games",
                    "media",
                ],
            },
        },
    },

    {
        id: "kids",
        name: "Kids Devices",
        tags: [
            "family",
        ],

        userIds: [
            "alice",
            "bob",
        ],

        agentSelections: {
            pihole: {
                deviceIds: [
                    "desktop",
                ],
                resourceIds: [
                    "internet",
                ],
            },

            esp32: {
                deviceIds: [
                    "switch",
                ],
                resourceIds: [
                    "power",
                    "network",
                ],
            },
        },
    },

    {
        id: "gaming",
        name: "Gaming Consoles",
        tags: [],

        userIds: [],

        agentSelections: {
            esp32: {
                deviceIds: [
                    "xbox",
                    "ps5",
                ],
                resourceIds: [
                    "power",
                    "network",
                ],
            },
        },
    },
];

//
// Users
//

export async function getUsers(): Promise<User[]> {
    return structuredClone(mockUsers);
}

export async function getUser(
    id: string,
): Promise<User | undefined> {
    return structuredClone(
        mockUsers.find((u) => u.id === id),
    );
}

//
// Wards
//

export async function getWards(): Promise<Ward[]> {
    return structuredClone(mockWards);
}

export async function getWard(
    id: string,
): Promise<Ward | undefined> {
    return structuredClone(
        mockWards.find((w) => w.id === id),
    );
}

export async function createWard(
    ward: Ward,
): Promise<void> {
    mockWards.push(structuredClone(ward));
}

export async function updateWard(
    ward: Ward,
): Promise<void> {
    const index = mockWards.findIndex(
        (w) => w.id === ward.id,
    );

    if (index >= 0) {
        mockWards[index] = structuredClone(ward);
    }
}

export async function deleteWard(
    id: string,
): Promise<void> {
    const index = mockWards.findIndex(
        (w) => w.id === id,
    );

    if (index >= 0) {
        mockWards.splice(index, 1);
    }
}