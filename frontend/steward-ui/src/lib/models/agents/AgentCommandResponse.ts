// not sure if this one is needed

export interface AgentCommandResponse {
    requestId: string;

    commandStatus: AgentCommandStatus;

    message?: string;
}

export enum AgentCommandStatus {
    Accepted = "Accepted",
    Completed = "Completed",
    Failed = "Failed"
}