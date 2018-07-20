# Responses

If a request succeeds and a stub is found, the configured response will be returned. There are several "response writers" within HttPlaceholder which can be used to arrange your response. These will be explained on this page.

## Text

To return a plain text respone, use the "text" response writer. If no Content-Type header is set, the header will be set to "text/plain";

```yml
- id: situation-03
  conditions:
    method: GET
    url:
      path: /text.txt
  response:
    statusCode: 200
    text: It works!
    headers:
      Content-Type: text/plain
```

## JSON

This is a shortcut for returning a JSON string. This response writer sets the "Content-Type" header to "application/json".

```yml
- id: situation-json
  conditions:
    method: GET
    url:
      path: /text.json
  response:
    statusCode: 200
    json: {"msg": "All OK!"}
```

## XML

This is a shortcut for returning an XML string. This response writer sets the "Content-Type" header to "text/xml".

```yml
- id: situation-json
  conditions:
    method: GET
    url:
      path: /text.json
  response:
    statusCode: 200
    xml: <xml></xml>
```

## HTML

This is a shortcut for returning an HTML string. This response writer sets the "Content-Type" header to "text/html".

```yml
- id: situation-json
  conditions:
    method: GET
    url:
      path: /index.html
  response:
    statusCode: 200
    xml: |
      <html>
        <head>
          <title>Test page in HttPlaceholder</title>
        </head>
        <body>
          <h1>Example in HttPlaceholder</h1>
          <p>
            Hey, this is just a proof of concept of a site created and hosted in HttPlaceholder. Works pretty good huh?
          </p>
        </body>
      </html>
```

## Status code

To set the HTTP status code of a response, use the "statusCode" response writer. If this is not set, the default will be used (which is 200 OK).

```yml
- id: situation-03
  conditions:
    method: GET
    url:
      path: /text.txt
  response:
    statusCode: 200
    text: It works!
    headers:
      Content-Type: text/plain
```

## Headers

To return a set of HTTP headers with your response, use the "headers" response writer.

```yml
- id: situation-03
  conditions:
    method: GET
    url:
      path: /text.txt
  response:
    statusCode: 200
    text: It works!
    headers:
      Content-Type: text/plain
      X-Correlation: correlation_id
```

## File

To return a file from disk, use the "file" response writer. There are two ways in which you can use this response writer.

### Scenario 1

If you don't specify the full path, HttPlaceholder will look in the same folder where your .yml file resides.

```yml
- id: image-file
  conditions:
    method: GET
    url:
      path: /cat_file.jpg
  response:
    statusCode: 200
    file: cat_file.jpg
    headers:
      Content-Type: image/jpeg
```

### Scenario 2

You can also use the full path to a file.

```yml
- id: image-file
  conditions:
    method: GET
    url:
      path: /cat_file.jpg
  response:
    statusCode: 200
    file: C:\files\cat_file.jpg
    headers:
      Content-Type: image/jpeg
```

## Base64

You can also specify a base64 string which should be decoded and returned by HttPlaceholder. You can use this if you want to encode a binary and paste it in your script.

```yml
- id: base64-example
  conditions:
    method: GET
    url:
      path: /text.txt
  response:
    statusCode: 200
    base64: SXQgd29ya3Mh
    headers:
      Content-Type: text/plain
      X-Correlation: correlation_id
```

## Extra duration

Whenever you want to simulate a busy web service, you can use the "extraDuration" response writer. You can set the number of extra milliseconds HttPlaceholder should wait and the request will actually take that much time to complete.

```yml
- id: slow
  conditions:
    method: GET
    url:
      path: /users
      query:
        id: 12
        filter: first_name
  response:
    statusCode: 200
    text: |
      {
        "first_name": "John"
      }
    extraDuration: 10000
    headers:
      Content-Type: application/json
```

## Permanent and temporary redirects

The permanent and temporary redirect response writers are short hands for defining redirects in you stub. If you set an URL on the "temporaryRedirect" property, HttPlaceholder will redirect the user with an HTTP 307, and when you use the "permanentRedirect" an HTTP 301.

```yml
- id: temp-redirect
  conditions:
    method: GET
    url:
      path: /temp-redirect
  response:
    temporaryRedirect: https://google.com
```

```yml
- id: permanent-redirect
  conditions:
    method: GET
    url:
      path: /permanent-redirect
  response:
    permanentRedirect: https://reddit.com
```