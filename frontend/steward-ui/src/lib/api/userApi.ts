import type { User } from "../models";
import { client } from "./client";

export async function getUsers(): Promise<User[]> {
    return client.get<User[]>("/users");
}

export async function getUser(
    id: string,
): Promise<User | undefined> {
    return client.get<User>(`/users/${id}`);
}

// Devices
export async function assignUserDevice(userId: number, deviceId: number): Promise<void> {
    return client.put<void>(`/users/${userId}/devices/${deviceId}`)
}

export async function removeUserDevice(userId: number, deviceId: number): Promise<void> {
    return client.delete<void>(`/users/${userId}/devices/${deviceId}`)
}
export type SaveUser = { name: string; type: "admin" | "member"; email: string | null; pin?: string; clearPin?: boolean };
export const createUser = (user: SaveUser) => client.post<User>("/users", user);
export const updateUser = (id: number, user: SaveUser) => client.put<User>(`/users/${id}`, user);
