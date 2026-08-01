export interface Agent {
    agentId: string;
    instanceId: string;

    name: string;

    status: AgentStatus;

    resourceIds: string[];

    lastSeen?: Date;
}

export enum AgentStatus {
    Offline = "Offline",
    Online = "Online",
    Disabled = "Disabled"
}