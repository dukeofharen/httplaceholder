#!/bin/bash
# Regex
curl --location --request GET "http://localhost:5000/host-2" --header "Host: httplaceholder2.local" -D-

# Full
curl --location --request GET "http://localhost:5000/host-1" --header "Host: httplaceholder.local" -D-