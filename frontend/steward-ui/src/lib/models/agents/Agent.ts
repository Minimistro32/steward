import type { Resource } from "./Resource";

export interface Agent {
    agentId: string;
    instanceId: string;

    name: string;

    status: AgentStatus;

    resources: Resource[];

    lastSeen?: Date;
}

export enum AgentStatus {
    Offline = "Offline",
    Online = "Online"
}