const handleResponse = (response) => {
  const headers = {};
  for (let header of response.headers.entries()) {
    headers[header[0]] = header[1];
  }

  const contentType = headers["content-type"];
  let isJson = false;
  if (contentType && contentType.includes("application/json")) {
    isJson = true;
  }

  if (!response.ok) {
    const error = new Error(response.statusText);
    error.json = isJson ? response.json() : response.text();
    throw error;
  }

  return isJson ? response.json() : response.text();
};

export function get(url, options) {
  options = options || {};
  return fetch(url, {
    method: "get",
    headers: options.headers || {},
  }).then(handleResponse);
}

export function del(url, options) {
  options = options || {};
  return fetch(url, {
    method: "delete",
    headers: options.headers || {},
  }).then(handleResponse);
}
