#!/bin/bash
# Exact IP
curl --location --request GET "http://localhost:5000/client-ip-1" -D-

# IP range
curl --location --request GET "http://localhost:5000/client-ip-2" -D-