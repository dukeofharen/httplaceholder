type BeforeSendHandler = { (url: string, request: RequestInit): void };
const beforeSendHandlers: BeforeSendHandler[] = [];

export interface RequestOptions {
  headers: object | undefined;
}

export interface PreparedRequest {
  body: string;
  contentType: string;
}

type Headers = {
  [key: string]: string;
};

export type HttpError = {
  statusText: string;
  body: any;
  status: number;
};

const handleResponse = async (response: Response) => {
  const headers: Headers = {};
  response.headers.forEach((value, key) => (headers[key] = value));
  const contentType = headers["content-type"];
  let isJson = false;
  if (contentType && contentType.includes("application/json")) {
    isJson = true;
  }

  if (!response.ok) {
    throw <HttpError>{
      body: isJson ? await response.json() : await response.text(),
      status: response.status,
      statusText: response.statusText,
    };
  }

  return isJson ? response.json() : response.text();
};

function prepareRequest(input: any): PreparedRequest {
  switch (typeof input) {
    case "string":
      return <PreparedRequest>{
        body: input,
        contentType: "text/plain",
      };
    case "object":
      return <PreparedRequest>{
        body: JSON.stringify(input),
        contentType: "application/json",
      };
    default:
      return <PreparedRequest>{
        body: "",
        contentType: "",
      };
  }
}

const handleBeforeSend = (url: string, request: RequestInit) => {
  for (const handler of beforeSendHandlers) {
    handler(url, request);
  }
};

export function addBeforeSendHandler(action: BeforeSendHandler) {
  beforeSendHandlers.push(action);
}

export function get(url: string, options?: RequestOptions) {
  options = options || {
    headers: {},
  };
  const request = <RequestInit>{
    method: "get",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}

export function del(url: string, options?: RequestOptions) {
  options = options || {
    headers: {},
  };
  const request = <RequestInit>{
    method: "delete",
    headers: options.headers || {},
  };
  handleBeforeSend(url, request);
  return fetch(url, request).then(handleResponse);
}

export function put(url: string, body: any, options?: RequestOptions) {
  const preparedRequest = prepareRequest(body);
  options = options || {
    headers: {},
  };
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

export function post(url: string, body: any, options?: RequestOptions) {
  const preparedRequest = prepareRequest(body);
  options = options || {
    headers: {},
  };
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
