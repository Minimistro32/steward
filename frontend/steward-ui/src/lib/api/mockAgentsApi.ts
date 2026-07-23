import type { Agent } from "../models/agents/Agent";
import { AgentStatus } from "../models/agents/Agent";
import type { Resource } from "../models/agents/Resource";

const resources: Resource[] = [
    {
        id: "media",
        name: "Media",
        actions: [
            "block"
        ]
    },
    {
        id: "games",
        name: "Games",
        actions: [
            "block",
            "allow"
        ]
    },
    {
        id: "browser",
        name: "Browser",
        actions: [
            "block"
        ]
    }
];


const agents: Agent[] = [
    {
        agentId: "desktop-alice",
        instanceId: "abc123",
        name: "Alice Desktop",
        status: AgentStatus.Online,
        resources: [
            resources[0],
            resources[1]
        ],
        lastSeen: new Date()
    },

    {
        agentId: "laptop-bob",
        instanceId: "def456",
        name: "Bob Laptop",
        status: AgentStatus.Online,
        resources: [
            resources[1],
            resources[2]
        ],
        lastSeen: new Date(Date.now() - 1000 * 60 * 5)
    },

    {
        agentId: "tablet-kids",
        instanceId: "ghi789",
        name: "Kids Tablet",
        status: AgentStatus.Offline,
        resources: [
            resources[0]
        ],
        lastSeen: new Date(Date.now() - 1000 * 60 * 60 * 12)
    }
];


export function getAgents(): Agent[] {
    return agents;
}


export function getAgent(agentId: string): Agent | undefined {
    return agents.find(
        (agent) => agent.agentId === agentId
    );
}


export function refreshAgents(): Agent[] {
    // Later this will call Steward.Server
    return agents;
}