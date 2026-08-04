import type { User, Ward } from "../models/wards";
import { client } from "./client";


//
// Users
//

export async function getUsers(): Promise<User[]> {
    return client.get<User[]>("/users");
}


export async function getUser(
    id: string,
): Promise<User | undefined> {
    return client.get<User>(`/users/${id}`);
}


//
// Wards
//

export async function getWards(): Promise<Ward[]> {
    return client.get<Ward[]>("/wards");
}


export async function getWard(
    id: string,
): Promise<Ward | undefined> {
    return client.get<Ward>(`/wards/${id}`);
}


export async function createWard(
    ward: Ward,
): Promise<Ward> {
    return client.post<Ward>(
        "/wards",
        ward,
    );
}


export async function updateWard(
    ward: Ward,
): Promise<void> {
    await client.put(
        `/wards/${ward.id}`,
        ward,
    );
}


export async function deleteWard(
    id: string,
): Promise<void> {
    await client.delete(
        `/wards/${id}`,
    );
}