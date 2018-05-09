#!/bin/bash
curl -v \
  --request PUT \
  --data '{"firstName":"John","lastName":"doe","age":26,"address":{"streetAddress":"naist street","city":"Nara","postalCode":"630-0192"},"phoneNumbers":[{"type":"iPhone","number":"0123-4567-8888"},{"type":"home","number":"0123-4567-8910"}]}' \
  --header "Content-Type: application/json" \
  "http://localhost:5000/users"
echo "=========="