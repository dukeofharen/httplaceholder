#!/bin/bash
# Regular
curl --location --request GET "http://localhost:5000/slooooow?id=12&filter=first_name" -D-

# Regular min/max
curl --location --request GET "http://localhost:5000/slooooow-min-max?id=12&filter=first_name" -D-