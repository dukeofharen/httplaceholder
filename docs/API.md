# HttPlaceholder REST API

Like many other automation and development tools, HttPlaceholder has a REST API that you can use to automate the creation of stubs. By default, the stubs and requests are stored in the `.httplaceholder` folder of the current logged in user (you can change this behavior; see [config](CONFIG.md)). The REST API gives you access to four collections: the stubs collection, the requests collection (to see all requests that are made to HttPlaceholder), users collection and tenants collection.

Click [here](https://github.com/dukeofharen/httplaceholder/releases/latest) if you want the swagger.json file. Using this swagger.json file, you can easily create a REST client for your favourite programming language (e.g. using a tool like [autorest](https://github.com/Azure/autorest)).

The [Postman collection](samples/requests.json) also contains REST API examples.

## General

The REST API accepts both JSON and YAML strings (when doing a POST or PUT). If you want to post a YAML string, set the `Content-Type` header to `application/x-yaml`, if you want to post a JSON string, set the `Content-Type` header to `application/json`. If you do a request where you expect a textual response, set the `Accept` header to `application/x-yaml` if you want to get YAML or `application/json` if you want to get JSON.

If you have enabled authentication (see [config](CONFIG.md) for more information), you also need to provide an `Authorization` header with the correct basic authentication. So if, for example, the username is `user` and the password is `pass`, the following value should be used for the `Authorization` header: `Basic dXNlcjpwYXNz`. For every call in the REST API, a `401 Unauthorized` is returned if the authentication is incorrect.

### Stubs

A stub is a combination of condition checkers and response writers that will be executed when a valid request is sent.

### Requests

Any kind of HTTP request that is made against HttPlaceholder. Also requests where no stub could be matched are saved, because you might want to create a stub based on that request.

### Users

A simple collection to check whether a given user is valid or not.

### Tenants

Tenants allow you to group your stubs. When you've assigned a "tenant" field to your stub (see [conditions](CONDITIONS.md) for more information), you can perform batch operations on a larger set of stubs. The tenants endpoint helps you with this.