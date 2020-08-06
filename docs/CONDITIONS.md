# Conditions

Whenever HttPlaceholder receives a request, all the conditions of all stubs are checked to see which stub corresponds to the sent request. There are condition checkers for example the URL, posted data etc. This page explains more.

## General

HttPlaceholder uses two different conditions: "regular conditions" and "negative conditions". Let's say we have the following stub .yml file:

```yml
- id: situation-03
  tenant: users-api
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

The stub also has a "tenant" field defined. This is a free text field which is optional. This field makes it possible to do operations of multiple stubs at once (e.g. delete all stubs with a specific tenant, get all stubs of a specific tenant or update all stubs of a specific tenant). To learn more about tenants, go to [API](API.md).

For both the conditions and negativeConditions, all condition checkers as explained on this page are available.

## Description

A free text field where you can specify where the stub is for. It is optional.

```yml
- id: situation-01
  description: Returns something
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

## Priority

There are cases when a request matches multiple stub. If this is the case, you can use the "priority" element. With the priority element, you can specify which stub should be used if multiple stubs are found. The stub with the highest priority will be used. If you don't set the priority on the stub, it will be 0 by default.

```yml
- id: fallback
  priority: -1
  conditions: 
    method: GET
  response:
    statusCode: 200
    text: OK-Fallback

- id: situation-01
  conditions:
    method: GET
    url:
      path: /users
  response:
    statusCode: 200
    text: OK
```

In the scenario above, if you got to url `http://httplaceholder/users`, both stubs will be matched. Because the priority of the fallback stub is -1, the other stub will be used instead.

## Path

The path condition is used to check a part of the URL path (so the part after http://... and before the query string). The condition can both check on substring and regular expressions.

```yml
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

```yml
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

```yml
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

```yml
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

```yml
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

## Is HTTPS

This condition checker can be used to verify if a request uses HTTPS or not. To configure HttPlaceholder with HTTPS, read [configuration](CONFIG.md) (hint: it's not hard at all).

```yml
- id: ishttps-ok
  conditions:
    method: GET
    url:
      path: /ishttps-ok
      isHttps: true
  response:
    statusCode: 200
    text: OK
```

**Correct request**
- Method: GET
- URL: https://localhost:5050/anyPath

## Basic authentication

This condition checker can check whether the sent basic authentication matches with the data in the stub.

```yml
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

```yml
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

```yml
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
- Headers:
    - Content-Type: application/x-www-form-urlencoded
- Body:
```json
{"username": "john"}
```

## Form

The form value condition checker can check whether the posted form values correspodn to the given rules in the stub. It is possible to add multiple conditions. The condition can both check on substring and regular expressions.

```yml
- id: form-ok
  conditions:
    method: POST
    url:
      path: /form
    form:
      - key: key1
        value: sjaak
      - key: key2
        value: bob
      - key: key2
        value: ducoo
  response:
    text: OK
```

**Correct request**
- Method: POST
- URL: http://localhost:5000/form
- Body:
```
key1=sjaak&key2=bob&key2=ducoo
```

## Client IP validation

It is also possible to set a condition to check the the client IP. A condition can be set for a single IP address or a whole IP range.

```yml
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

```yml
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

## Hostname

It is possible to check if a hostname in a request is correct. This condition can be used with regular expressions if needed.

```yml
# Check the hostname on full name.
- id: host-1
  conditions:
    method: GET
    host: httplaceholder.com
  response:
    statusCode: 200
    text: OK
```

```yml
- id: host-2
  conditions:
    method: GET
    host: http(.*)
  response:
    statusCode: 200
    text: OK
```

## JSONPath

Using the JSONPath condition checker, you can check the posted JSON body to see if it contains the correct elements. It is possible to add multiple conditions.

```yml
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
```json
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

```yml
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

```yml
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
```xml
<?xml version="1.0"?><object><a>TEST</a><b>TEST2</b></object>
```