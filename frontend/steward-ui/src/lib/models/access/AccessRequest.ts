export interface AccessRequest {
    reason?: string | null;
    policyId: number;
    requestedMinutes: number;
}