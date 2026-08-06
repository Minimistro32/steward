import type { AccessOption, User } from "../models";
import { client } from "./client";

export async function getUsers(): Promise<User[]> {
    return client.get<User[]>("/users");
}

export async function getUser(
    id: string,
): Promise<User | undefined> {
    return client.get<User>(`/users/${id}`);
}

// Access
export async function getAccessOptions(userId: number): Promise<AccessOption[]> {
    type AccessResponse = { options: AccessOption[]; }
    return (await client.get<AccessResponse>(`/users/${userId}/access`)).options ?? [];
}

export async function postAccessRequest(userId: number) {
    // return await client.post<>(`/users/${userId}/access/request`)
}

// Devices
export async function assignUserDevice(userId: number, deviceId: number): Promise<void> {
    return client.put<void>(`/users/${userId}/devices/${deviceId}`)
}

export async function removeUserDevice(userId: number, deviceId: number): Promise<void> {
    return client.delete<void>(`/users/${userId}/devices/${deviceId}`)
}