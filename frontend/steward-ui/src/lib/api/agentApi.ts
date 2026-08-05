import type { Agent } from "../models";
import { client } from "./client";


export async function getAgents(): Promise<Agent[]> {
    return client.get<Agent[]>("/agents");
}


export async function refreshAgents(): Promise<void> {
    await client.post<void>("/agents/refresh");
}