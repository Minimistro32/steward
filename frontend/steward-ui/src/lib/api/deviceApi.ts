import type { Device } from "../models";
import { client } from "./client";

export async function getDevices(): Promise<Device[]> {
    return client.get<Device[]>("/device");
}
