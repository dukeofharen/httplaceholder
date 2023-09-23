#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# JSONPath 1
curl --location --request PUT "$HTTPL_ROOT_URL/users" \
--header "Content-Type: application/json' \
--data-raw '{
    "firstName": "John",
    "lastName": "doe",
    "age": 26,
    "address": {
        "streetAddress": "naist street",
        "city": "Nara",
        "postalCode": "630-0192"
    },
    "phoneNumbers": [
        {
            "type": "iPhone",
            "number": "0123-4567-8888"
        }
    ]
}' -D-

# JSONPath 2
curl --location --request PUT '$HTTPL_ROOT_URL/users' \
--header 'Content-Type: application/json' \
--data-raw '{
    "firstName": "John",
    "lastName": "doe",
    "age": 26,
    "address": {
        "streetAddress": "naist street",
        "city": "Nara",
        "postalCode": "630-0192"
    },
    "phoneNumbers": [
        {
            "type": "home",
            "number": "0123-4567-8910"
        }
    ]
}' -D-

# JSONPath 3
curl --location --request PUT '$HTTPL_ROOT_URL/users' \
--header 'Content-Type: application/json' \
--data-raw '{
    "firstName": "John",
    "lastName": "doe",
    "age": 26,
    "address": {
        "streetAddress": "naist street",
        "city": "Nara",
        "postalCode": "630-0192"
    },
    "phoneNumbers": [
        {
            "type": "iPhone",
            "number": "0123-4567-8910"
        }
    ]
}' -D-

# JSON array
curl --location --request POST '$HTTPL_ROOT_URL/json' \
--header 'Content-Type: application/json' \
--data-raw '[
    "val1",
    3,
    1.46,
    "2021-04-17T13:16:54",
    {
        "stringVal": "val1",
        "intVal": 55
    }
]' -D-

# JSON object
curl --location --request POST '$HTTPL_ROOT_URL/json' \
--header 'Content-Type: application/json' \
--data-raw '{
  "username": "username",
  "subObject": {
    "strValue": "stringInput",
    "boolValue": true,
    "doubleValue": 1.23,
    "dateTimeValue": "2021-04-16T21:23:03",
    "intValue": 3,
    "nullValue": null,
    "arrayValue": [
      "val1",
      {
        "subKey1": "subValue1",
        "subKey2": "subValue2"
      }
    ]
  }
}' -D-