#!/bin/bash
git remote add all git@github.com:dukeofharen/httplaceholder.git
git remote set-url --add all git@gitlab.com:ducode/httplaceholder.git

# Now you can push with git push all --all