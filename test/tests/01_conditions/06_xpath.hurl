# Create stub: XPath namespaces specified
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: xml-with-namespaces-specified
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-xml-with-namespaces
  headers:
    Content-Type: application/soap+xml; charset=utf-8
  xpath:
    - queryString: /soap:Envelope/soap:Body/m:GetUser/m:Username[text() = 'user1']
      namespaces:
        soap: http://www.w3.org/2003/05/soap-envelope
        m: http://www.example.org/stock/GetUser
response:
  text: OK XML WITH NAMESPACES SPECIFIED
```

HTTP 200

# Create stub: XPath namespaces not specified
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: xml-without-namespaces-specified
tenant: integration
conditions:
  method: POST
  url:
    path:
      equals: /post-xml-without-namespaces
  headers:
    Content-Type: application/soap+xml; charset=utf-8
  xpath:
    - queryString: /soap:Envelope/soap:Body/m:GetUser/m:Username[text() = 'user1']
response:
  text: OK XML WITHOUT NAMESPACES SPECIFIED
```

HTTP 200

# Check the request: with namespaces
POST {{rootUrl}}/post-xml-with-namespaces
Content-Type: application/soap+xml; charset=utf-8
```
<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" 
    xmlns:m="http://www.example.org/stock/GetUser">
    <soap:Header></soap:Header>
    <soap:Body>
        <m:GetUser>
            <m:Username>user1</m:Username>
        </m:GetUser>
    </soap:Body>
</soap:Envelope>
```

HTTP 200
[Asserts]
body equals "OK XML WITH NAMESPACES SPECIFIED"

# Check the request: with namespaces and XPath incorrect
POST {{rootUrl}}/post-xml-with-namespaces
Content-Type: application/soap+xml; charset=utf-8
```
<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" 
    xmlns:m="http://www.example.org/stock/GetUser">
    <soap:Header></soap:Header>
    <soap:Body>
        <m:GetUser>
            <m:Username>user2</m:Username>
        </m:GetUser>
    </soap:Body>
</soap:Envelope>
```

HTTP 200
[Asserts]
body equals "OK FALLBACK"

# Check the request: without namespaces
POST {{rootUrl}}/post-xml-without-namespaces
Content-Type: application/soap+xml; charset=utf-8
```
<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" 
    xmlns:m="http://www.example.org/stock/GetUser">
    <soap:Header></soap:Header>
    <soap:Body>
        <m:GetUser>
            <m:Username>user1</m:Username>
        </m:GetUser>
    </soap:Body>
</soap:Envelope>
```

HTTP 200
[Asserts]
body equals "OK XML WITHOUT NAMESPACES SPECIFIED"

# Check the request: without namespaces and XPath incorrect
POST {{rootUrl}}/post-xml-without-namespaces
Content-Type: application/soap+xml; charset=utf-8
```
<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" 
    xmlns:m="http://www.example.org/stock/GetUser">
    <soap:Header></soap:Header>
    <soap:Body>
        <m:GetUser>
            <m:Username>user2</m:Username>
        </m:GetUser>
    </soap:Body>
</soap:Envelope>
```

HTTP 200
[Asserts]
body equals "OK FALLBACK"