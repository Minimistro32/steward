import type { Device } from "../";
import type { Resource } from "./Resource";

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