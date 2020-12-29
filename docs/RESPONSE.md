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
    html: |
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

## Content type

Instead of setting a header with the content type, you can also use the `contentType` property. This will always override the value set in the response headers.

```yml
- id: content-type-csv
  conditions:
    method: GET
    url:
      path: /content-type.csv
  response:
    statusCode: 200
    text: 'id,name,amount\n1,DukeOfHaren,20'
    contentType: text/csv
```

## Image

It is possible for HttPlaceholder to generate stub images. This can be done by setting the `image` response writer. This writer is built using the amazing [ImageSharp](https://docs.sixlabors.com/) library. Here is an example:

```yml
id: image-example
conditions:
  method: GET
  url:
    path: /image.png
response:
  image:
    type: png
    width: 1024
    height: 256
    backgroundColor: '#ffa0d3'
    text: Placeholder text that will be drawn in the image
    fontSize: 10
    jpegQuality: 95
    wordWrap: false
```

When you visit the URL `http://localhost:5000/image.png`, you'll get this image:

![](img/image_example_1.png)

The following properties can be set for the `image` response writer:

- `type`: the image type that should be returned. Can be `jpeg`, `png`, `bmp` or `gif`.
- `width`: the image width in pixels.
- `height`: the image height in pixels.
- `backgroundColor`: the HEX value of the background color. By providing an additional transparency value, you can specify the transparency of the background. So, if you take the example above, `#ffa0d3ff` means the background is fully visible and `#ffa0d300` means the background is fully transparent.
- `text`: the text that should be drawn in the image.
- `fontSize`: the size of the drawn text.
- `fontColor`: the HEX value of the text color. If you do not set this value, the text color will be the inverted value of `backgroundColor`, but since inverted grayscales might produce roughly the same color, you can also specify your own color here.
- `jpegQuality`: of course, only useful if `type` `jpeg` is used. Is `95` by default. `1` is worst and `100` is best quality.
- `wordWrap`: if set to true, the text will be written across the image. Useful if your text is long. Is `false` by default.

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

## Update line endings

In some cases, you might want to enforce which types of line endings are returned. Some software might only react correctly on Windows or Unix line endings. Besides that, GIT might change the line endings when you commit your stub file, so it is not always clear what the actual line endings are. For setting the line endings specifically, the `lineEndings` response writer was introduced. The supported values are `unix` and `windows`. Here are 2 examples:

*Enforce Unix line endings*

```yml
- id: line-endings-all-unix
  conditions:
    method: GET
    url:
      path: /unix-line-endings
  response:
    lineEndings: unix
    text: |
      text
      with
      unix
      line
      endings
```

*Enforce Windows line endings*

```yml
- id: line-endings-all-windows
  conditions:
    method: GET
    url:
      path: /windows-line-endings
  response:
    lineEndings: windows
    text: |
      text
      with
      windows
      line
      endings
```

## Dynamic mode

In order to make the responses in HttPlaceholder a bit more dynamic, the "dynamic mode" was introduced. This makes it possible to add variables to your responses that can be parsed. As of now, these variables can be used in the response body (text only) and the response headers. The only requirement is that you set the response variable `enableDynamicMode` to true (by default, it is set to false and the variables will not be parsed).

Variables are written like this `((function_name))` or `((function_name:input))`.

```yml
- id: dynamic-query-example
  conditions:
    method: GET
    url:
      path: /dynamic.txt
  response:
    enableDynamicMode: true
    headers:
      X-Header: ((uuid)) ((uuid))
    text: ((uuid)) ((uuid)) ((uuid))
```

### Query string

The query string parser makes it possible to write request query string parameters to the response.

```yml
- id: dynamic-query-example-txt
  conditions:
    method: GET
    url:
      path: /dynamic-query.txt
  response:
    enableDynamicMode: true
    headers:
      X-Header: ((query:response_header))
    text: ((query:response_text))
```

Let's say you make the request `http://localhost:5000/dynamic-query.txt?response_text=RESPONSE!&response_header=HEADER!`. `((query:response_header))` will be replaced with `RESPONSE!` and `((query:response_text))` will be replaced with `HEADER!`. If no matching query paramter was found, the variable will be filled with an empty string.

```yml
- id: dynamic-encoded-query-example-txt
  conditions:
    method: GET
    url:
      path: /dynamic-encoded-query.txt
  response:
    enableDynamicMode: true
    headers:
      X-Header: ((query_encoded:response_header))
    text: ((query_encoded:response_text))
```

The example above is roughly the same, but writes the query parameter URL encoded.

### UUID

The UUID parser makes it possible to insert a random UUID to the response.

```yml
- id: dynamic-uuid-example
  conditions:
    method: GET
    url:
      path: /dynamic-uuid.txt
  response:
    enableDynamicMode: true
    text: ((uuid))
    headers:
      X-Header: ((uuid))
  priority: 0
```

If you go to `http://localhost:5000/dynamic-uuid.txt`, you will retrieve random UUID as response content and a random UUID in the `X-Header` response header.

### Request headers

The request headers parser makes it possible to write request header values to the response.

```yml
- id: dynamic-request-header-example
  conditions:
    method: GET
    url:
      path: /dynamic-request-header.txt
  response:
    enableDynamicMode: true
    text: 'API key: ((request_header:X-Api-Key))'
    headers:
      X-Header: ((request_header:Host))
  priority: 0
```

Let's say you make the request `http://localhost:5000/dynamic-request-header.txt` with header `X-Api-Key: api123`. `((request_header:X-Api-Key))` will be replaced with `api123` and `((request_header:Host))` will be replaced with the hostname (e.g. `localhost:5000`). If no matching request header was found, the variable will be filled with an empty string.

### Form post

The form post parser makes it possible to write posted form values to the response.

```yml
- id: dynamic-form-post-example
  conditions:
    method: POST
    url:
      path: /dynamic-form-post.txt
  response:
    enableDynamicMode: true
    text: 'Posted: ((form_post:formval1))'
    headers:
      X-Header: ((form_post:formval2))
  priority: 0
```

Let's say you make the request `http://localhost:5000/dynamic-form-post.txt` with the following data:

**Posted body**
```
formval1=value1&formval2=value2
```

**Headers**
`Content-Type`: `application/x-www-form-urlencoded`

`((form_post:formval1))` will be replaced with `value1` and `((form_post:formval2))` will be replaced with `value2`.

### Request body

The request body parser makes it possible to write the complete posted body to the response.

```yml
- id: dynamic-request-body-example
  conditions:
    method: POST
    url:
      path: /dynamic-request-body.txt
  response:
    enableDynamicMode: true
    text: 'Posted: ((request_body))'
    headers:
      X-Header: ((request_body))
  priority: 0
```

Let's say you make the request `http://localhost:5000dynamic-request-body.txt` with the following data:

**Posted body**
```
Test123
```

`((request_body))` will be replaced with `Test123`.

### Display URL

The display URL body parser makes it possible to write the complete URL to the response.

```yml
- id: dynamic-display-url-example
  conditions:
    method: GET
    url:
      path: /dynamic-display-url.txt
  response:
    enableDynamicMode: true
    text: 'URL: ((display_url))'
    headers:
      X-Header: ((display_url))
  priority: 0
```

Let's say you do the following GET request: `http://localhost:5000/dynamic-display-url.txt?var1=value&var2=value2`. The response text will look like this:

```
URL: http://localhost:5000/dynamic-display-url.txt?var1=value&var2=value2
```

### Client IP

The client IP body parser makes it possible to write the IP address of the requester to the response.

```yml
- id: dynamic-client-ip-example
  conditions:
    method: GET
    url:
      path: /dynamic-client-ip.txt
  response:
    enableDynamicMode: true
    text: 'IP: ((client_ip))'
    headers:
      X-Header: ((client_ip))
  priority: 0
```

Let's say you make the following request:

**URL**
```
http://localhost:5000/dynamic-client-ip.txt
```

**Method**
```
GET
```

**IP**
```
192.168.178.15
```

The response will look like this:
```
IP: 192.168.178.15
```

### Local and UTC date & time

These two body parsers can insert the current local date & time or the UTC date & time in the response body or headers.

```
- id: dynamic-local-now-example
  conditions:
      method: GET
      url:
          path: /dynamic-local-now.txt
  response:
      enableDynamicMode: true
      text: 'Local now: ((localnow:yyyy-MM-dd HH:mm:ss))'
      headers:
          X-Header: ((localnow:yyyy-MM-dd HH:mm:ss))
  priority: 0

- id: dynamic-utc-now-example
  conditions:
      method: GET
      url:
          path: /dynamic-utc-now.txt
  response:
      enableDynamicMode: true
      text: 'UTC now: ((utcnow:yyyy-MM-dd HH:mm:ss))'
      headers:
          X-Header: ((utcnow:yyyy-MM-dd HH:mm:ss))
  priority: 0
```

**URL**
```
http://localhost:5000/dynamic-local-now-ip.txt
```

**Method**
```
GET
```

The response will look like this:
```
Local now: 2019-08-21 21:13:59
```

For both `localnow` and `utcnow`, you can optionally provide a date format. Since HttPlaceholder is a .NET application, you can use all DateTime format strings supported by .NET. For information on all formatting strings, read https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings.

## Reverse proxy

HttPlaceholder contains a very simple reverse proxy as response option. In short, if you want to route some requests (or any request that doesn't match any other stub) to an external web service, you can do this with the reverse proxy response writer. Here are some examples that you can use.

Scenario: `https://jsonplaceholder.typicode.com/todos` is an API for testing purposes which returns a list of todo items. We want to configure HttPlaceholder to proxy the requests to this URL and return the response of that call to the client.

```
- id: reverse-proxy-1
  conditions:
    url:
      path: /todos
  response:
    reverseProxy:
      url: https://jsonplaceholder.typicode.com/todos
      appendPath: true
      appendQueryString: true
      replaceRootUrl: true
```

When you now make a call to `http://localhost:5000/todos`, a request will be made to `https://jsonplaceholder.typicode.com/todos` and the response of that call will be returned to the client.

The variable `appendPath` is set to true (which is, by default, set to false by the way), which means that everything you put in your URL after `/todos` (which you've you configured in your conditions) will be appended to the proxy URL. So, let's say you go to `http://localhost:5000/todos/1`, HttPlaceholder will send a request to `https://jsonplaceholder.typicode.com/todos/1`.

Also, the variable `appendQueryString` is set to true (which is by default false). Like the name says, it appends the query string of the request to HttPlaceholder to the reverse proxy request. For example, let's say you make a request to `http://localhost:5000/todos?key=val`, then HttPlaceholder will make a request to `https://jsonplaceholder.typicode.com/todos?key=val`.

Finally, there is also a reverse proxy setting called `replaceRootUrl` (which is by default false). If this is set to true, any reference of `https://jsonplaceholder.typicode.com` (so the **root** URL of your reverse proxy URL) will be replaced by the root URL of HttPlaceholder (e.g. `http://localhost:5000`). The replacing will be done in the reverse proxy response body and response headers.