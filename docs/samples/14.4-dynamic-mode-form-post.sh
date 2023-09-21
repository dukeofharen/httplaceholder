#!/bin/bash
curl --location --request POST "http://localhost:5000/dynamic-form-post.txt" \
--header "Content-Type: application/x-www-form-urlencoded" \
--data-raw "formval1=value 1&formval2=value 2' -D-