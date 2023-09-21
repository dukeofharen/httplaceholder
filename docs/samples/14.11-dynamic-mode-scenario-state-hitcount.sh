#!/bin/bash
# Set scenario
curl --location --request PUT 'http://localhost:5000/ph-api/scenarios/scenario123' \
--header 'Content-Type: application/json' \
--data-raw '{
    "state": "cool_state_1",
    "hitCount": 10
}'

# State
curl --location --request GET 'http://localhost:5000/dynamic-mode-scenario-state.txt' -D-

# Hit count
curl --location --request GET 'http://localhost:5000/dynamic-mode-scenario-hitcount.txt' -D-