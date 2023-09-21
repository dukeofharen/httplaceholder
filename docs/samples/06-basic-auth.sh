#!/bin/bash
curl --location --request GET 'http://localhost:5000/auth?id=12&filter=first_name' \
--header 'Authorization: Basic dXNlcjpwYXNz' -D-