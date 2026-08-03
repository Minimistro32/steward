import {
    type Agent,
    AgentStatus,
} from "../models/agents";

const mockAgents: Agent[] = [
    {
        agentId: "esp32",
        instanceId: "abc123",
        name: "ESP32 Relay",
        status: AgentStatus.Online,

        devices: [
            {
                id: "switch",
                name: "Nintendo Switch",
            },
            {
                id: "xbox",
                name: "Xbox",
            },
            {
                id: "ps5",
                name: "PlayStation 5",
            },
        ],

        resources: [
            {
                id: "power",
                name: "Power",
            },
            {
                id: "network",
                name: "Network",
            },
        ],

        lastSeen: new Date(),
    },

    {
        agentId: "pihole",
        instanceId: "def456",
        name: "Pi-hole",
        status: AgentStatus.Disabled,

        devices: [
            {
                id: "desktop",
                name: "Family Desktop",
            },
        ],

        resources: [
            {
                id: "internet",
                name: "Internet",
            },
            {
                id: "browser",
                name: "Browser",
            },
        ],

        lastSeen: new Date(Date.now() - 1000 * 60 * 5),
    },

    {
        agentId: "coldTurkey",
        instanceId: "ghi789",
        name: "Cold Turkey",
        status: AgentStatus.Offline,

        devices: [
            {
                id: "gaming-pc",
                name: "Gaming PC",
            },
            {
                id: "laptop",
                name: "Laptop",
            },
        ],

        resources: [
            {
                id: "media",
                name: "Media",
            },
            {
                id: "games",
                name: "Games",
            },
        ],

        lastSeen: new Date(Date.now() - 1000 * 60 * 60 * 12),
    },
];

//
// Agents
//

export async function getAgents(): Promise<Agent[]> {
    return structuredClone(mockAgents);
}

export async function getAgent(
    agentId: string,
): Promise<Agent | undefined> {
    return structuredClone(
        mockAgents.find((a) => a.agentId === agentId),
    );
}

export async function refreshAgents(): Promise<Agent[]> {
    // Later this will call Steward.Server
    return structuredClone(mockAgents);
}