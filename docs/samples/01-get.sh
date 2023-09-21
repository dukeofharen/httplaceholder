#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh

# Regular 1
curl --location --request GET "http://localhost:5000/users?id=12&filter=first_name" -D-

# Regular 2
curl --location --request GET "http://localhost:5000/users?id=14&filter=last_name" -D-

# Use fullPath
curl --location --request GET "http://localhost:5000/users?filter=first_name" -D-

# Fallback scenario
curl --location --request GET "http://localhost:5000/users" -D-