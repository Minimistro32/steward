export interface AgentCommand {
    requestId: string;

    resourceId: string;

    action: string;

    expiresAt?: Date;
}