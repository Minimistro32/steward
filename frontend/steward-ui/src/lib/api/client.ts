import { setSession, sessionRevision } from "../session";
const API_URL = "/api";
export class ApiError extends Error {
    constructor(public status: number, public errors: string[] = []) { super(`API Error: ${status}`); }
}

async function request<T>(
    path: string,
    options?: RequestInit,
): Promise<T> {
    const revision = sessionRevision();
    const response = await fetch(`${API_URL}${path}`, {
        credentials: "include",
        headers: {
            "X-Steward-Request": "1",
            "Content-Type": "application/json",
        },
        ...options,
    });

    if (!response.ok) {
        if (response.status === 401 && path !== "/auth/login" && revision === sessionRevision()) setSession(null);
        const body = await response.json().catch(() => null);
        const errors = body?.errors ? Object.values(body.errors).flat().filter((value): value is string => typeof value === "string") : [];
        throw new ApiError(response.status, errors);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return await response.json();
}


export const client = {
    get<T>(path: string) {
        return request<T>(path);
    },

    post<T>(path: string, body?: unknown) {
        return request<T>(path, {
            method: "POST",
            body: JSON.stringify(body),
        });
    },

    put<T>(path: string, body?: unknown) {
        return request<T>(path, {
            method: "PUT",
            body: JSON.stringify(body),
        });
    },

    delete<T>(path: string) {
        return request<T>(path, {
            method: "DELETE",
        });
    },
};