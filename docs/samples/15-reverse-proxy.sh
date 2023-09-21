#!/bin/bash
# Regular: get all todo items
curl --location --request GET "http://localhost:5000/todoitems/todos" -D-

# Regular: get todo item 1
curl --location --request GET "http://localhost:5000/todoitems/todos/1" -D-

# Regular: get todo item 2
curl --location --request GET "http://localhost:5000/todoitems/todos/2" -D-

# Regular: query string forwarding
curl --location --request GET "http://localhost:5000/itemsproxy?id=14" -D-