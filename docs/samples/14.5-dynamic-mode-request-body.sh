#!/bin/bash
# Regex
curl --location --request POST "http://localhost:5000/dynamic-request-body-regex.txt' \
--header 'Content-Type: text/plain' \
--data-raw 'key1=value1
key2=value2
key3=value3' -D-

# Regular
curl --location --request POST 'http://localhost:5000/dynamic-request-body.txt' \
--header 'Content-Type: text/plain' \
--data-raw 'TEST VALUE' -D-