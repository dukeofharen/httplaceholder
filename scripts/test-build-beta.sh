#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
export APPVEYOR_BUILD_NUMBER="1234"
export APPVEYOR_REPO_COMMIT_MESSAGE="[beta] New version"
export APPVEYOR_REPO_BRANCH="dev"
. $DIR/build.sh