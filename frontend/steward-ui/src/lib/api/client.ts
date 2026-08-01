const API_URL = "http://localhost:8080/api";

async function request<T>(path: string): Promise<T> {
    const response = await fetch(`${API_URL}${path}`);

    if (!response.ok) {
        throw new Error(`API Error: ${response.status}`);
    }

    return await response.json();
}

export const apiClient = {
    get: request,
};