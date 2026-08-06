export interface OverrideRequest {
    id: string;

    policyId: string;

    wardId: string;

    requestedAt: Date;

    status: "Pending" | "Approved" | "Denied";

    requestedMinutes: number;
}