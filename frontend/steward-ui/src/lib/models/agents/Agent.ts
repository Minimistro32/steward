import type { Device, Resource } from "..";

export interface Agent {
    id: number
    agentId: string;
    instanceId: string;
    name: string;
    status: AgentStatus;
    resources: Resource[];
    devices: Device[];
    lastContact?: Date;
}

export enum AgentStatus {
    Offline = "Offline",
    Online = "Online",
    Disabled = "Disabled"
}