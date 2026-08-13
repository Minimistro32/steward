export interface Ward {
    id?: number;
    name: string;
    tags: string[];

    userIds: number[];
    deviceIds: number[];
    resourceIds: number[]
}