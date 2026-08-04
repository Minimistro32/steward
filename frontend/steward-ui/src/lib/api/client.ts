const API_URL = "http://localhost:5042/api";

async function request<T>(
    path: string,
    options?: RequestInit,
): Promise<T> {
    const response = await fetch(`${API_URL}${path}`, {
        headers: {
            "Content-Type": "application/json",
        },
        ...options,
    });

    if (!response.ok) {
        throw new Error(`API Error: ${response.status}`);
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