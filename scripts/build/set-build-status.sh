#!/bin/bash
apt update
apt install curl -y
STATE="${1-failure}"
DESCRIPTION=""
if [ "$STATE" = "failure" ] || [ "$STATE" = "error" ]; then
  DESCRIPTION="The build failed."
fi
if [ "$STATE" = "pending" ]; then
  DESCRIPTION="The build is pending."
fi
if [ "$STATE" = "success" ]; then
  DESCRIPTION="The build succeeded."
fi

curl \
  -X POST \
  -H "Accept: application/vnd.github+json" \
  -H "Authorization: token $GITHUB_API_KEY" \
  https://api.github.com/repos/dukeofharen/httplaceholder/statuses/$CI_COMMIT_SHA \
  -d "{\"state\":\"$STATE\",\"target_url\":\"https://gitlab.com/ducode/httplaceholder/-/pipelines/$CI_PIPELINE_ID\",\"description\":\"$DESCRIPTION\",\"context\":\"continuous-integration/gitlab-ci\"}"