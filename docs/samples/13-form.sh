#!/bin/bash
curl --location --request POST "http://localhost:5000/form" \
--header "Content-Type: application/x-www-form-urlencoded" \
--data-urlencode "key1=sjaak" \
--data-urlencode "key2=bob" \
--data-urlencode "key2=ducoo" -D-