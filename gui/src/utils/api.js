const beforeSendHandlers = [];

const handleResponse = async (response) => {
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
    error.body = isJson ? await response.json() : await response.text();
    error.status = response.status;
    throw error;
  }

  return isJson ? response.json() : response.text();
};

const prepareRequest = (input) => {
  switch (typeof input) {
    case "string":
      return {
        body: input,
        contentType: "text/plain",
      };
    case "object":
      return {
        body: JSON.stringify(input),
        contentType: "application/json",
      };
    default:
      return {
        body: "",
        contentType: "",
      };
  }
};

const handleBeforeSend = (url, request) => {
  for (const handler of beforeSendHandlers) {
    handler(url, request);
  }
};

export function addBeforeSendHandler(action) {
  beforeSendHandlers.push(action);
}

export function get(url, options) {
  options = options || {};
  const request = {
    method: "get",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}

export function del(url, options) {
  options = options || {};
  const request = {
    method: "delete",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}

export function put(url, body, options) {
  const preparedRequest = prepareRequest(body);
  options = options || {};
  const headers = Object.assign(
    { "content-type": preparedRequest.contentType },
    options.headers || {}
  );
  const request = {
    method: "put",
    headers,
    body: preparedRequest.body,
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}

export function post(url, body, options) {
  const preparedRequest = prepareRequest(body);
  options = options || {};
  const headers = Object.assign(
    { "content-type": preparedRequest.contentType },
    options.headers || {}
  );
  const request = {
    method: "post",
    headers,
    body: preparedRequest.body,
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}
