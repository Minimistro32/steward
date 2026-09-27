import type { OverrideRequirement } from "../policies";

export interface RequestActivity {
    id: number;
    userId: number;
    userName: string;
    requirement: OverrideRequirement | null;
    status: OverrideRequestStatus;
    createdAt: string;
    requestedMinutes: number;
    resources: string[];
}

export type OverrideRequestStatus =
    | "pending"
    | "granted"
    | "rejected";