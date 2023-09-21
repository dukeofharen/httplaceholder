#!/bin/bash
curl --location --request GET "http://localhost:5000/dynamic-request-header.txt' \
--header 'X-Api-Key: abc123' -D-