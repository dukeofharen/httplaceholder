#!/bin/bash
# Local
curl --location --request GET "http://localhost:5000/dynamic-local-now.txt' -D-

# UTC
curl --location --request GET 'http://localhost:5000/dynamic-utc-now.txt' -D-