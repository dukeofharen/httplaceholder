#!/bin/bash
curl -v \
  --header "Authorization: Basic dXNlcjpwYXNz" \
  "http://localhost:5000/users?id=12&filter=first_name"
echo "=========="