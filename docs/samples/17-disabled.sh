#!/bin/bash
# Enabled
curl --location --request GET 'http://localhost:5000/enabled' -D-

# Disabled
curl --location --request GET 'http://localhost:5000/disabled' -D-