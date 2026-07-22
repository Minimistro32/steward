import type { User } from "./User";
import type { Device } from "./Device";

export interface Ward {
    id: string;

    name: string;

    tags: string[];

    users: User[];

    devices: Device[];

    resources: string[];
}