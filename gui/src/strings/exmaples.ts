export const exampleCurlInput = `curl 'https://api.site.com/api/v1/users' \\
  -X 'PUT' \\
  -H 'authority: api.site.com' \\
  -H 'sec-ch-ua: " Not A;Brand";v="99", "Chromium";v="96", "Google Chrome";v="96"' \\
  -H 'accept: application/json, text/plain, */*' \\
  -H 'content-type: application/json;charset=UTF-8' \\
  -H 'authorization: Bearer VERYLONGSTRING \\
  -H 'sec-ch-ua-mobile: ?0' \\
  -H 'user-agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36' \\
  -H 'sec-ch-ua-platform: "Linux"' \\
  -H 'origin: https://site.com' \\
  -H 'sec-fetch-site: same-site' \\
  -H 'sec-fetch-mode: cors' \\
  -H 'sec-fetch-dest: empty' \\
  -H 'accept-language: en-US,en;q=0.9,nl;q=0.8' \\
  --data-raw $'{"id":1,"created":"2015-10-21T14:39:55","updated":"2021-11-26T22:10:52","userName":"d","firstName":"d\\'","lastName":"h","street":"Road","number":"6","postalCode":"1234AB","city":"Amsterdam","phone":"0612345678","email":"info@example.com","placeId":1,"newsletter":false,"driversLicenseNumber":"112233","emailRepeat":"info@example.com"}' \\
  --compressed`;
export const exampleHarInput = `{
  "log": {
    "version": "1.2",
    "creator": {
      "name": "Firefox",
      "version": "95.0.1"
    },
    "browser": {
      "name": "Firefox",
      "version": "95.0.1"
    },
    "pages": [
      {
        "startedDateTime": "2021-12-29T22:37:23.266+01:00",
        "id": "page_2",
        "title": "Example Domain",
        "pageTimings": {
          "onContentLoad": 232,
          "onLoad": 249
        }
      }
    ],
    "entries": [
      {
        "pageref": "page_2",
        "startedDateTime": "2021-12-29T22:37:23.266+01:00",
        "request": {
          "bodySize": 0,
          "method": "GET",
          "url": "http://example.com/",
          "httpVersion": "HTTP/1.1",
          "headers": [
            {
              "name": "Host",
              "value": "example.com"
            },
            {
              "name": "User-Agent",
              "value": "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:95.0) Gecko/20100101 Firefox/95.0"
            },
            {
              "name": "Accept",
              "value": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8"
            },
            {
              "name": "Accept-Language",
              "value": "en-US,en;q=0.5"
            },
            {
              "name": "Accept-Encoding",
              "value": "gzip, deflate"
            },
            {
              "name": "Connection",
              "value": "keep-alive"
            },
            {
              "name": "Upgrade-Insecure-Requests",
              "value": "1"
            },
            {
              "name": "DNT",
              "value": "1"
            },
            {
              "name": "Sec-GPC",
              "value": "1"
            },
            {
              "name": "Pragma",
              "value": "no-cache"
            },
            {
              "name": "Cache-Control",
              "value": "no-cache"
            }
          ],
          "cookies": [],
          "queryString": [],
          "headersSize": 404
        },
        "response": {
          "status": 200,
          "statusText": "OK",
          "httpVersion": "HTTP/1.1",
          "headers": [
            {
              "name": "Accept-Ranges",
              "value": "bytes"
            },
            {
              "name": "Age",
              "value": "581800"
            },
            {
              "name": "Cache-Control",
              "value": "max-age=604800"
            },
            {
              "name": "Content-Type",
              "value": "text/html; charset=UTF-8"
            },
            {
              "name": "Date",
              "value": "Wed, 29 Dec 2021 21:37:23 GMT"
            },
            {
              "name": "Etag",
              "value": "\\"3147526947\\""
            },
            {
              "name": "Expires",
              "value": "Wed, 05 Jan 2022 21:37:23 GMT"
            },
            {
              "name": "Last-Modified",
              "value": "Thu, 17 Oct 2019 07:18:26 GMT"
            },
            {
              "name": "Server",
              "value": "ECS (dcb/7EA7)"
            },
            {
              "name": "Vary",
              "value": "Accept-Encoding"
            },
            {
              "name": "X-Cache",
              "value": "HIT"
            }
          ],
          "cookies": [],
          "content": {
            "mimeType": "text/html; charset=UTF-8",
            "size": 1256,
            "text": "<!doctype html>\\n<html>\\n<head>\\n    <title>Example Domain</title>\\n\\n    <meta charset=\\"utf-8\\" />\\n    <meta http-equiv=\\"Content-type\\" content=\\"text/html; charset=utf-8\\" />\\n    <meta name=\\"viewport\\" content=\\"width=device-width, initial-scale=1\\" />\\n    <style type=\\"text/css\\">\\n    body {\\n        background-color: #f0f0f2;\\n        margin: 0;\\n        padding: 0;\\n        font-family: -apple-system, system-ui, BlinkMacSystemFont, \\"Segoe UI\\", \\"Open Sans\\", \\"Helvetica Neue\\", Helvetica, Arial, sans-serif;\\n        \\n    }\\n    div {\\n        width: 600px;\\n        margin: 5em auto;\\n        padding: 2em;\\n        background-color: #fdfdff;\\n        border-radius: 0.5em;\\n        box-shadow: 2px 3px 7px 2px rgba(0,0,0,0.02);\\n    }\\n    a:link, a:visited {\\n        color: #38488f;\\n        text-decoration: none;\\n    }\\n    @media (max-width: 700px) {\\n        div {\\n            margin: 0 auto;\\n            width: auto;\\n        }\\n    }\\n    </style>    \\n</head>\\n\\n<body>\\n<div>\\n    <h1>Example Domain</h1>\\n    <p>This domain is for use in illustrative examples in documents. You may use this\\n    domain in literature without prior coordination or asking for permission.</p>\\n    <p><a href=\\"https://www.iana.org/domains/example\\">More information...</a></p>\\n</div>\\n</body>\\n</html>\\n"
          },
          "redirectURL": "",
          "headersSize": 374,
          "bodySize": 1022
        },
        "cache": {},
        "timings": {
          "blocked": 0,
          "dns": 0,
          "connect": 95,
          "ssl": 0,
          "send": 0,
          "wait": 99,
          "receive": 0
        },
        "time": 194,
        "_securityState": "insecure",
        "serverIPAddress": "93.184.216.34",
        "connection": "80"
      },
      {
        "pageref": "page_2",
        "startedDateTime": "2021-12-29T22:37:23.548+01:00",
        "request": {
          "bodySize": 0,
          "method": "GET",
          "url": "http://example.com/favicon.ico",
          "httpVersion": "HTTP/1.1",
          "headers": [
            {
              "name": "Host",
              "value": "example.com"
            },
            {
              "name": "User-Agent",
              "value": "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:95.0) Gecko/20100101 Firefox/95.0"
            },
            {
              "name": "Accept",
              "value": "image/avif,image/webp,*/*"
            },
            {
              "name": "Accept-Language",
              "value": "en-US,en;q=0.5"
            },
            {
              "name": "Accept-Encoding",
              "value": "gzip, deflate"
            },
            {
              "name": "Referer",
              "value": "http://example.com/"
            },
            {
              "name": "Connection",
              "value": "keep-alive"
            },
            {
              "name": "DNT",
              "value": "1"
            },
            {
              "name": "Sec-GPC",
              "value": "1"
            },
            {
              "name": "Pragma",
              "value": "no-cache"
            },
            {
              "name": "Cache-Control",
              "value": "no-cache"
            }
          ],
          "cookies": [],
          "queryString": [],
          "headersSize": 355
        },
        "response": {
          "status": 404,
          "statusText": "Not Found",
          "httpVersion": "HTTP/1.1",
          "headers": [
            {
              "name": "Content-Encoding",
              "value": "gzip"
            },
            {
              "name": "Accept-Ranges",
              "value": "bytes"
            },
            {
              "name": "Age",
              "value": "578211"
            },
            {
              "name": "Cache-Control",
              "value": "max-age=604800"
            },
            {
              "name": "Content-Type",
              "value": "text/html; charset=UTF-8"
            },
            {
              "name": "Date",
              "value": "Wed, 29 Dec 2021 21:37:23 GMT"
            },
            {
              "name": "Expires",
              "value": "Wed, 05 Jan 2022 21:37:23 GMT"
            },
            {
              "name": "Last-Modified",
              "value": "Thu, 23 Dec 2021 05:00:32 GMT"
            },
            {
              "name": "Server",
              "value": "ECS (dcb/7F80)"
            },
            {
              "name": "Vary",
              "value": "Accept-Encoding"
            },
            {
              "name": "X-Cache",
              "value": "404-HIT"
            },
            {
              "name": "Content-Length",
              "value": "648"
            }
          ],
          "cookies": [],
          "content": {
            "mimeType": "text/html; charset=UTF-8",
            "size": 1256,
            "text": "<!doctype html>\\n<html>\\n<head>\\n    <title>Example Domain</title>\\n\\n    <meta charset=\\"utf-8\\" />\\n    <meta http-equiv=\\"Content-type\\" content=\\"text/html; charset=utf-8\\" />\\n    <meta name=\\"viewport\\" content=\\"width=device-width, initial-scale=1\\" />\\n    <style type=\\"text/css\\">\\n    body {\\n        background-color: #f0f0f2;\\n        margin: 0;\\n        padding: 0;\\n        font-family: -apple-system, system-ui, BlinkMacSystemFont, \\"Segoe UI\\", \\"Open Sans\\", \\"Helvetica Neue\\", Helvetica, Arial, sans-serif;\\n        \\n    }\\n    div {\\n        width: 600px;\\n        margin: 5em auto;\\n        padding: 2em;\\n        background-color: #fdfdff;\\n        border-radius: 0.5em;\\n        box-shadow: 2px 3px 7px 2px rgba(0,0,0,0.02);\\n    }\\n    a:link, a:visited {\\n        color: #38488f;\\n        text-decoration: none;\\n    }\\n    @media (max-width: 700px) {\\n        div {\\n            margin: 0 auto;\\n            width: auto;\\n        }\\n    }\\n    </style>    \\n</head>\\n\\n<body>\\n<div>\\n    <h1>Example Domain</h1>\\n    <p>This domain is for use in illustrative examples in documents. You may use this\\n    domain in literature without prior coordination or asking for permission.</p>\\n    <p><a href=\\"https://www.iana.org/domains/example\\">More information...</a></p>\\n</div>\\n</body>\\n</html>\\n"
          },
          "redirectURL": "",
          "headersSize": 365,
          "bodySize": 1013
        },
        "cache": {},
        "timings": {
          "blocked": 0,
          "dns": 0,
          "connect": 0,
          "ssl": 0,
          "send": 0,
          "wait": 100,
          "receive": 0
        },
        "time": 100,
        "_securityState": "insecure",
        "serverIPAddress": "93.184.216.34",
        "connection": "80"
      }
    ]
  }
}`;
export const exampleOpenApiInput = `openapi: "3.0.0"
info:
  version: 1.0.0
  title: Swagger Petstore
  license:
    name: MIT
servers:
  - url: http://petstore.swagger.io/v1
paths:
  /pets:
    get:
      summary: List all pets
      operationId: listPets
      tags:
        - pets
      parameters:
        - name: limit
          in: query
          description: How many items to return at one time (max 100)
          required: false
          schema:
            type: integer
            format: int32
        - name: X-Api-Key
          in: header
          description: API key.
          schema:
            type: string
      responses:
        '200':
          description: A paged array of pets
          headers:
            x-next:
              description: A link to the next page of responses
              schema:
                type: string
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Pets"
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Error"
    post:
      summary: Create a pet
      operationId: createPets
      tags:
        - pets
      responses:
        '201':
          description: Null response
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Error"
  /pets/{petId}:
    get:
      summary: Info for a specific pet
      operationId: showPetById
      tags:
        - pets
      parameters:
        - name: petId
          in: path
          required: true
          description: The id of the pet to retrieve
          schema:
            type: string
      responses:
        '200':
          description: Expected response to a valid request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Pet"
        default:
          description: unexpected error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Error"
components:
  schemas:
    Pet:
      type: object
      required:
        - id
        - name
      properties:
        id:
          type: integer
          format: int64
        name:
          type: string
        tag:
          type: string
    Pets:
      type: array
      items:
        $ref: "#/components/schemas/Pet"
    Error:
      type: object
      required:
        - code
        - message
      properties:
        code:
          type: integer
          format: int32
        message:
          type: string
`;
export const defaultStub = `id: unique-stub-id
description: A description for the stub.
conditions:
  method: GET
response:
  text: OK!`;
