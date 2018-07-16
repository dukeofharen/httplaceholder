# Conditions

Whenever HttPlaceholder receives a request, all the conditions of all stubs are checked to see which stub corresponds to the sent request. There are condition checkers for example the URL, posted data etc. This page explains more.

## General

HttPlaceholder uses two different conditions: "regular conditions" and "negative conditions". Let's say we have the following stub .yml file:

```
- id: situation-03
  conditions:
    method: GET
    url:
      path: /users
      query:
        id: 15
        filter: last_name
  negativeConditions:
    url:
      query:
        last_name: Johnson
  response:
    statusCode: 200
    text: |
      {
        "last_name": "Jackson"
      }
    headers:
      Content-Type: application/json
```

This example uses both conditions and negativeConditions. This means that when:
- The URL path contains "/users"
- A query string with name "id" and value "15" is sent.
- A query string with name "filter" and value "last_name" is sent.
- There is **no** query string with name "last_name" and value "Johnson".

If all these (negative) conditions match, the response as defined under the "response" element is returned. For more information about the response element, you can read more [here](RESPONSE.md).

For both the conditions and negativeConditions, all condition checkers as explained on this page are available.

## Path

The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions.

```
- id: situation-01
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users/1

```
- id: situation-01
  conditions:
    method: GET
    url:
      # Now with regex. Path should exactly match /users in this case.
      path: ^/users$
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users

## Full path

This condition checker looks a lot like the path checker, but this checker also checks extra URL parameters, like the query string. The condition can both check on substring and regular expressions.

```
- id: situation-01
  conditions:
    method: GET
    url:
      fullPath: /users?filter=first_name
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/users?filter=first_name

## Query string

This condition checker can check the query string in a name-value collection like way. The condition can both check on substring and regular expressions.

```
- id: situation-01
  conditions:
    method: GET
    url:
      query:
        id: 14
        filter: last_name
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath?id=14&filter=last_name

## Method

This condition checker can check the HTTP method (e.g. GET, POST, PUT, DELETE etc.).

```
- id: situation-01
  conditions:
    method: GET
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath

## Basic authentication

This condition checker can check whether the sent basic authentication matches with the data in the stub.

```
- id: basic-auth
  conditions:
    method: GET
    basicAuthentication:
      username: user
      password: pass
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath
- Headers:
    - Authorization: Basic dXNlcjpwYXNz

## Header

This condition checker can check whether the sent headers match with the headers in the stub. The condition can both check on substring and regular expressions.

```
- id: basic-auth
  conditions:
    method: GET
    headers:
      X-Api-Key: secret123
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: http://localhost:5000/anyPath
- Headers:
    - X-Api-Key: secret123

## Body

This condition checker can check whether the posted body corresponds to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions.

```
- id: situation-01
  conditions:
    method: POST
    url:
      path: \busers\b
    body:
      - \busername\b
      - \bjohn\b
  response:
    statusCode: 200
    text: '{"result": true}'
    headers:
      Content-Type: application/json
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/users
- Body:
```
{"username": "john"}
```

## Client IP validation

It is also possible to set a condition to check the the client IP. A condition can be set for a single IP address or a whole IP range.

```
# Client IP address validation on a single IP address
- id: client-ip-1
  conditions:
    method: GET
    url:
      path: /client-ip-1
    clientIp: 127.0.0.1
  response:
    statusCode: 200
    text: OK
```

```
# Client IP address validation on an IP range
- id: client-ip-2
  conditions:
    method: GET
    url:
      path: /client-ip-2
    clientIp: '127.0.0.0/29'
  response:
    statusCode: 200
    text: OK
```

## JSONPath

Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions.

```
- id: jpath-test
  conditions:
    method: PUT
    url:
      path: /users
    jsonPath:
      - "$.phoneNumbers[?(@.type=='iPhone')]"
  response:
    statusCode: 204
```

**Correct request**
- Method: PUT
- URL: http://localhost:5000/users
- Body:
```
{
	"phoneNumbers": [{
		"type": "iPhone",
		"number": "0123-4567-8888"
	}, {
		"type": "home",
		"number": "0123-4567-8910"
	}]
}
```

## XPath

Using the XPath condition checker, you can check the posted XML body to see if it contains the correct elements. It is possible to add multiple conditions.

It is also possible to (pre)-set the XML namespaces of a posted XML body. If no namespaces are set in the stub, HttPlaceholder will try to fetch the namespaces itself using a regular expression.

```
- id: regular-xml
  conditions:
    method: POST
    url:
      path: /thingy
    headers:
      Content-Type: application/soap+xml; charset=utf-8
    xpath:
      - queryString: /object/a[text() = 'TEST']
  response:
    statusCode: 200
    text: <result>OK</result>
    headers:
      Content-Type: text/xml
```

```
- id: regular-xml
  conditions:
    method: POST
    url:
      path: /thingy
    headers:
      Content-Type: application/soap+xml; charset=utf-8
    xpath:
      - queryString: /object/a[text() = 'TEST']
        namespaces:
          soap: http://www.w3.org/2003/05/soap-envelope
          m: http://www.example.org/stock/Reddy
  response:
    statusCode: 200
    text: <result>OK</result>
    headers:
      Content-Type: text/xml
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/thingy
- Headers:
    - Content-Type: application/soap+xml; charset=utf-8
- Body:
```
<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>
```