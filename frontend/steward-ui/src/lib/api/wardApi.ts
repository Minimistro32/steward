import type { Device, User, Ward } from "../models/wards";

const mockDevices: Device[] = [
    {
        id: "gaming-pc",
        name: "Gaming PC",
        agentId: "coldTurkey"
    },
    {
        id: "laptop",
        name: "Laptop",
        agentId: "coldTurkey"
    },
    {
        id: "desktop",
        name: "Family Desktop",
        agentId: "pihole"
    },
    {
        id: "switch",
        name: "Nintendo Switch",
        agentId: "esp32"
    },
    {
        id: "xbox",
        name: "Xbox",
        agentId: "esp32"
    },
    {
        id: "ps5",
        name: "PlayStation 5",
        agentId: "esp32"
    }
];

const mockUsers: User[] = [
    {
        id: "alice",
        name: "Alice",
        deviceIds: ["gaming-pc", "laptop"]
    },
    {
        id: "bob",
        name: "Bob",
        deviceIds: ["desktop", "switch"]
    },
    {
        id: "charlie",
        name: "Charlie",
        deviceIds: []
    }
];

const mockWards: Ward[] = [
    {
        id: "alice",
        name: "Alice",
        tags: ["gaming", "school"],
        userIds: ["alice"],
        deviceIds: ["gaming-pc", "laptop"],
        resourceIds: ["Steam", "Discord", "YouTube"]
    },
    {
        id: "kids",
        name: "Kids Devices",
        tags: ["family"],
        userIds: ["alice", "bob"],
        deviceIds: ["desktop", "switch"],
        resourceIds: ["Internet", "Minecraft", "Discord"]
    },
    {
        id: "gaming",
        name: "Gaming Consoles",
        tags: [],
        userIds: [],
        deviceIds: ["xbox", "ps5"],
        resourceIds: ["Power", "Network"]
    }
];

//
// Users
//

export async function getUsers(): Promise<User[]> {
    return structuredClone(mockUsers);
}

export async function getUser(id: string): Promise<User | undefined> {
    return structuredClone(
        mockUsers.find(u => u.id === id)
    );
}

//
// Devices
//

export async function getDevices(): Promise<Device[]> {
    return structuredClone(mockDevices);
}

export async function getDevice(id: string): Promise<Device | undefined> {
    return structuredClone(
        mockDevices.find(d => d.id === id)
    );
}

//
// Wards
//

export async function getWards(): Promise<Ward[]> {
    return structuredClone(mockWards);
}

export async function getWard(id: string): Promise<Ward | undefined> {
    return structuredClone(
        mockWards.find(w => w.id === id)
    );
}

export async function createWard(ward: Ward): Promise<void> {
    mockWards.push(structuredClone(ward));
}

export async function updateWard(ward: Ward): Promise<void> {
    const index = mockWards.findIndex(w => w.id === ward.id);

    if (index >= 0) {
        mockWards[index] = structuredClone(ward);
    }
}

export async function deleteWard(id: string): Promise<void> {
    const index = mockWards.findIndex(w => w.id === id);

    if (index >= 0) {
        mockWards.splice(index, 1);
    }
}