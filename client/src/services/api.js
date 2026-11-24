const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

let getAccessToken = () => null;
let updateAccessToken = () => {};

let refreshPromise = null;

export function setAccessTokenGetter(callback) {
  getAccessToken = callback;
}

export function setAccessTokenUpdater(callback) {
  updateAccessToken = callback;
}

async function refreshAccessToken() {
  if (refreshPromise) return refreshPromise;

  refreshPromise = (async () => {
    const response = await fetch(`${API_BASE_URL}/api/Auth/refresh`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) throw new Error("Session expired. Please login again.");

    const data = await response.json();

    updateAccessToken(data.accessToken);

    return data.accessToken;
  })();

  try {
    return await refreshPromise;
  } finally {
    refreshPromise = null;
  }
}

async function sendRequest(endpoint, fetchOptions, headers, token) {
  return fetch(`${API_BASE_URL}${endpoint}`, {
    ...fetchOptions,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...headers,
      ...(token && {
        Authorization: `Bearer ${token}`,
      }),
    },
  });
}

export async function apiRequest(endpoint, options = {}) {
  if (!API_BASE_URL) throw new Error("VITE_API_BASE_URL belum dikonfigurasi");

  const { accessToken, headers = {}, ...fetchOptions } = options;

  const currentToken = getAccessToken() ?? accessToken;

  let response = await sendRequest(
    endpoint,
    fetchOptions,
    headers,
    currentToken,
  );

  const normalizedEndpoint = endpoint.toLowerCase();

  const isAuthEndpoint = [
    "/auth/login",
    "/auth/register",
    "/auth/refresh",
    "/auth/logout",
  ].some((path) => normalizedEndpoint.endsWith(path));

  if (response.status === 401 && !isAuthEndpoint) {
    try {
      const newAccessToken = await refreshAccessToken();

      response = await sendRequest(
        endpoint,
        fetchOptions,
        headers,
        newAccessToken,
      );
    } catch (error) {
      updateAccessToken(null);
      throw error;
    }
  }

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
