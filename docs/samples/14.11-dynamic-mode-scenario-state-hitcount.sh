#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Set scenario
curl --location --request PUT '$HTTPL_ROOT_URL/ph-api/scenarios/scenario123' \
--header 'Content-Type: application/json' \
--data-raw '{
    "state": "cool_state_1",
    "hitCount": 10
}'

# State
curl --location --request GET '$HTTPL_ROOT_URL/dynamic-mode-scenario-state.txt' -D-

# Hit count
curl --location --request GET '$HTTPL_ROOT_URL/dynamic-mode-scenario-hitcount.txt' -D-