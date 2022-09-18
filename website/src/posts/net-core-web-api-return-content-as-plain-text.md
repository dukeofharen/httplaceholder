---
title: .NET (Core) Web API return content as plain text
date: 2018-03-31T20:30:11+02:00
description: .NET (Core) Web API return content as plain text
---

Hi,

Whenever I'm working on a WebAPI solution, I always return to the problem on "how to let WebAPI return the content as plain text". The answer to this solution is posted on [StackOverflow](https://stackoverflow.com/questions/11581697/is-there-a-way-to-force-asp-net-web-api-to-return-plain-text).

```
[HttpGet]
public HttpResponseMessage HelloWorld()
{
    string result = "Hello world! Time is: " + DateTime.Now;
    var resp = new HttpResponseMessage(HttpStatusCode.OK);
    resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
    return resp;
}
```

In the code above, a response message is created with which you can specify what kind of content type should be returned.

ASP.NET Core WebAPI is a lot smarter in this regard, because strings are handled as plain text by default:

```
public IActionResult Get(int id)
{
    string result = "Hello world! Time is: " + DateTime.Now;
    return Ok(result);
}
```