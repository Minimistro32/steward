import type { Device } from "./Device";
import type { Resource } from "./Resource";

export interface Agent {
    agentId: string;
    instanceId: string;
    name: string;
    status: AgentStatus;
    resources: Resource[];
    devices: Device[];
    lastSeen?: Date;
}

export enum AgentStatus {
    Offline = "Offline",
    Online = "Online",
    Disabled = "Disabled"
}