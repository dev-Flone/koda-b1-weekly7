const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export async function apiRequest(endpoint, options = {}) {
  if (!API_BASE_URL) throw new Error("VITE_API_BASE_URL belum dikonfigurasi");

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);

    throw new Error(
      error?.detail || error?.title || `API request failed: ${response.status}`,
    );
  }

  if (response.status === 204) return null;

  return response.json();
}

export function unwrapCollection(data) {
  const items = data?.value ?? data?.Value ?? data;

  return Array.isArray(items) ? items : [];
}
