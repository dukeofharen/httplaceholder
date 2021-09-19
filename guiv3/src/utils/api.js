const handleResponse = (response) => {
  if (!response.ok) {
    const error = new Error(response.statusText);
    error.json = response.json();
    throw error;
  }

  return response.json();
};

export function get(url, options) {
  options = options || {};
  return fetch(url, {
    method: "get",
    headers: options.headers || {},
  }).then(handleResponse);
}
