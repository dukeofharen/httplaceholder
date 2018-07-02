#!/bin/bash
curl -v \
  --request POST \
  --data '{"username": "john"}' \
  --header "X-Api-Key: 123abc" \
  --header "X-Another-Secret: 72354deg" \
  "http://localhost:5000/users"
echo "=========="