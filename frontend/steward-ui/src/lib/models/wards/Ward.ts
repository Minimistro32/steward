export interface Ward {
    id: string;
    name: string;
    tags: string[];

    userIds: string[];
    agentSelections: Record<string, {
        deviceIds: string[];
        resourceIds: string[];
    }>;
}