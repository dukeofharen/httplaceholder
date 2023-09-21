#!/bin/bash
# Temporary
curl --location --request GET 'http://localhost:5000/temp-redirect'

# Permanent
curl --location --request GET 'http://localhost:5000/permanent-redirect' -D-