# Getting started with HttPlaceholder

On this page, a very simple sample of HttPlaceholder is shown. If you want to try some more sophisticated samples, go to the page [Samples](SAMPLES.md).

## Simple HTTP GET example

Create a new file, called `stubs.yml` and add the following contents:

```yml
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

Start HttPlaceholder with the following arguments from the terminal:

```bash
httplaceholder --inputFile C:\path\to\stubs.yml
```

Otherwise, run the following command if you're already in the correct folder in your terminal:

```bash
httplaceholder
```

Both commands will start HttPlaceholder with the .yml file loaded.

If you then go to http://localhost:5000/users?id=12&filter=first_name, you'll get the response as defined in the .yml file. The HTTP port defined in the URL can be configured using command line arguments (for more information about the command line arguments, [read more here](CONFIG.md)).