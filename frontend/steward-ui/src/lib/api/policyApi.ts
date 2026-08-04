import type { Policy } from "../models/policies";
import { client } from "./client";


export async function getPolicies(): Promise<Policy[]> {
    return client.get<Policy[]>("/policies");
}


export async function getPolicy(
    id: string,
): Promise<Policy | undefined> {
    return client.get<Policy>(`/policies/${id}`);
}


export async function createPolicy(
    policy: Omit<Policy, "id" | "createdAt" | "modifiedAt">,
): Promise<Policy> {
    return client.post<Policy>(
        "/policies",
        policy,
    );
}


export async function updatePolicy(
    policy: Policy,
): Promise<void> {
    await client.put(
        `/policies/${policy.id}`,
        policy,
    );
}


export async function deletePolicy(
    id: string,
): Promise<void> {
    await client.delete(
        `/policies/${id}`,
    );
}