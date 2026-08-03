export interface User {
    id: string;
    name: string;
    agentSelections: Record<string, {
        deviceIds: string[];
    }>;
}