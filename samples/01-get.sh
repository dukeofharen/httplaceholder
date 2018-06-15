#!/bin/bash
curl -v "http://localhost:5000/users?id=12&filter=first_name"
echo "=========="

curl -v "http://localhost:5000/users?id=14&filter=last_name"
echo "=========="

curl -v "http://localhost:5000/users?id=15&filter=last_name&last_name=Johnson"
echo "=========="

curl -v "http://localhost:5000/users?id=18"
echo "=========="

curl -v "http://localhost:5000/users?filter=first_name"
echo "=========="