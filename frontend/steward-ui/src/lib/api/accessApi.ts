import type {
    AccessOption,
    AccessRequest,
    AccessResponse,
    OverrideAction,
} from "../models";

import { client } from "./client";

export async function getAccessOptions(userId: number): Promise<AccessOption[]> {
    return (await client.get<{ options: AccessOption[] }>(`/access/${userId}`)).options ?? [];
}

export async function postAccessRequest(
    userId: number,
    request: AccessRequest,
): Promise<AccessResponse> {
    return client.post<AccessResponse>(
        `/access/${userId}/request`,
        request,
    );
}


export async function postOverrideRequest(
    userId: number,
    request: AccessRequest,
): Promise<AccessResponse> {
    return client.post<AccessResponse>(
        `/access/${userId}/override`,
        request,
    );
}


export async function completeOverrideRequest(
    requestId: number,
    action: OverrideAction,
): Promise<AccessResponse> {
    return client.post<AccessResponse>(
        `/access/requests/${requestId}/complete`,
        action,
    );
}


export async function approveOverrideRequest(
    requestId: number,
    action: OverrideAction,
): Promise<AccessResponse> {
    return client.post<AccessResponse>(
        `/access/requests/${requestId}/approve`,
        action,
    );
}


export async function rejectOverrideRequest(
    requestId: number,
    action: OverrideAction,
): Promise<AccessResponse> {
    return client.post<AccessResponse>(
        `/access/requests/${requestId}/reject`,
        action,
    );
}