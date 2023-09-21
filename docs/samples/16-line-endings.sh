#!/bin/bash
# Enforce Unix line endings
curl --location --request GET "http://localhost:5000/unix-line-endings" -D-

# Enforce Windows line endings
curl --location --request GET "http://localhost:5000/windows-line-endings" -D-