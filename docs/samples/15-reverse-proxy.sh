#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
. $DIR/setup.sh
# Regular: get all todo items
curl --location --request GET "$HTTPL_ROOT_URL/todoitems/todos" -D-

# Regular: get todo item 1
curl --location --request GET "$HTTPL_ROOT_URL/todoitems/todos/1" -D-

# Regular: get todo item 2
curl --location --request GET "$HTTPL_ROOT_URL/todoitems/todos/2" -D-

# Regular: query string forwarding
curl --location --request GET "$HTTPL_ROOT_URL/itemsproxy?id=14" -D-