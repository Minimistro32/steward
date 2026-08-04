import type { Agent } from "../models";
import { client } from "./client";


export async function getAgents(): Promise<Agent[]> {
    return client.get<Agent[]>("/agents");
}


// export async function getAgent(
//     agentId: string,
// ): Promise<Agent | undefined> {
//     // We don't have GET /agents/{id}
//     // so filter client-side for now.

//     const agents = await getAgents();

//     return agents.find(
//         agent => agent.agentId === agentId,
//     );
// }


export async function refreshAgents(): Promise<void> {
    await client.post<void>("/agents/refresh");
}