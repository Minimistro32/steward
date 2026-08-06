import type { AccessOption, AccessResponse, User } from "../models";
import { client } from "./client";

export async function getUsers(): Promise<User[]> {
    return client.get<User[]>("/users");
}

export async function getUser(
    id: string,
): Promise<User | undefined> {
    return client.get<User>(`/users/${id}`);
}

export async function getAccessOptions(
    userId: number,
): Promise<AccessOption[]> {
    return (await client.get<AccessResponse>(`/users/${userId}/access`)).options ?? [];
}

export async function assignUserDevice(id: number, deviceId: number): Promise<void> {
    return client.put<void>(`/users/${id}/devices/${deviceId}`)
}

export async function removeUserDevice(id: number, deviceId: number): Promise<void> {
    return client.delete<void>(`/users/${id}/devices/${deviceId}`)
}