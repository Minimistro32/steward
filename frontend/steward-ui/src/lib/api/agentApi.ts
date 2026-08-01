import {
    type Agent,
    type Resource,
    AgentStatus
} from "../models/agents";

const mockResources: Resource[] = [
    {
        id: "media",
        name: "Media"
    },
    {
        id: "games",
        name: "Games"
    },
    {
        id: "browser",
        name: "Browser"
    }
];

const mockAgents: Agent[] = [
    {
        agentId: "esp32",
        instanceId: "abc123",
        name: "ESP32 Relay",
        status: AgentStatus.Online,
        resourceIds: [
            "media",
            "games"
        ],
        lastSeen: new Date()
    },

    {
        agentId: "pihole",
        instanceId: "def456",
        name: "Pi-hole",
        status: AgentStatus.Disabled,
        resourceIds: [
            "games",
            "browser"
        ],
        lastSeen: new Date(Date.now() - 1000 * 60 * 5)
    },

    {
        agentId: "coldTurkey",
        instanceId: "ghi789",
        name: "Cold Turkey",
        status: AgentStatus.Offline,
        resourceIds: [
            "media"
        ],
        lastSeen: new Date(Date.now() - 1000 * 60 * 60 * 12)
    }
];

//
// Resources
//

export async function getResources(): Promise<Resource[]> {
    return structuredClone(mockResources);
}

export async function getResource(id: string): Promise<Resource | undefined> {
    return structuredClone(
        mockResources.find(r => r.id === id)
    );
}

//
// Agents
//

export async function getAgents(): Promise<Agent[]> {
    return structuredClone(mockAgents);
}

export async function getAgent(agentId: string): Promise<Agent | undefined> {
    return structuredClone(
        mockAgents.find(a => a.agentId === agentId)
    );
}

export async function refreshAgents(): Promise<Agent[]> {
    // Later this will call Steward.Server
    return structuredClone(mockAgents);
}