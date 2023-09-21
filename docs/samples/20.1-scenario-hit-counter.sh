#!/bin/bash
# Min hits
curl --location --request GET 'http://localhost:5000/min-hits' -D-

# Max hits
curl --location --request GET 'http://localhost:5000/max-hits' -D-

# Exact hits
curl --location --request GET 'http://localhost:5000/exact-hits' -D-