# Placeholder

| | |
| --- | --- |
| **Build** | [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/49hfasb3vg1r8l8e?svg=true)](https://ci.appveyor.com/project/dukeofharen/placeholder) |
| **License** | [![MIT License](https://img.shields.io/:license-mit-green.svg)](https://opensource.org/licenses/MIT) |

Quickly mock away any webservice using Placeholder. Placeholder lets you specify what the request should look like and what response needs to be returned.

# Where?
[Download the latest release](https://github.com/dukeofharen/placeholder/releases/latest)

# Why?
At my current job, we develop and maintain a lot of web applications. These web applications have a lot of dependencies on other web services. So I thought: instead of running all these dependent web services on the local dev machine, why not use a stub to make development easier? That's exactly what I did.

So a few bullet points in a row:
1. Define all the needed requests in a YAML file, which can also be checked in for all your team members to use.
1. Many request conditions take regular expressions, to make your stub even more flexible.
1. (In the near future) use the Placeholder REST API to prepare all the requests you need for automated tests.

# How?
You have an input YAML file (which contains 1 or more requests):

```
- id: situation-01
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
    headers:
      Content-Type: application/json
```

In the `conditions` element, you specify which conditions the request should apply to.
- The method should be `GET`.
- The query parameters `id` and `filter` should be there with the values `12` and `first_name` respectively.

The `response` element defines the response of the request. In this case, HTTP 200 is returned, the literal JSON string defined and `application/json` as Content-Type header. For more examples, see the `samples` folder in this repository.

# Batteries included
- XPath conditions.
- JSONPath conditions.
- Request header conditions.
- Many conditions also take regular expressions.
- Basic authentication conditions.
- Add Base64 and file references to your response, so returning files is also possible.
- Artificially make your responses slower, to simulate slow web servers and test the timeout settings of your application.
- Placeholder can be run under HTTPS.

# Documentation
TODO

# Todo
- Make application "really" cross platform (Ubuntu and other Linux distributions, Mac etc.)
- Add REST API to add request definitions; useful in automated test scenario's.
- Let Placeholder run on IIS (already possible as reverse proxy, but IIS has native .NET Core support).