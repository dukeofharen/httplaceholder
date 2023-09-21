#!/bin/bash
curl --location --request POST 'http://localhost:5000/users' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--header 'X-Api-Key: 123abc' \
--header 'X-Another-Secret: 72354deg' \
--data-urlencode 'username=john' -D-