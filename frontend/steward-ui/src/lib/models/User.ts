export type User = {
    id: number;
    name: string;
    type: "admin" | "member";
    email: string | null;
    hasPin: boolean;
    deviceIds: number[];
}