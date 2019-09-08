#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
SONAR_KEY="f875d24735ce0d5f67dcca834407fb451e960f22"
PROJECT_KEY="HttPlaceholderAPI"
SONAR_URL="http://localhost:9000"

cd ../../src
dotnet $DIR/SonarScanner.MSBuild.dll begin \
    /k:"$PROJECT_KEY" \
    /d:sonar.host.url="$SONAR_URL" \
    /d:sonar.login="$SONAR_KEY" \
    /d:sonar.exclusions=**/*.js,**/*.css,**/*.less,**/*.scss,**/HttPlaceholder.TestConsoleApp/**
dotnet build --no-incremental
# find . -name "*.Tests.csproj" | while read fname; do
#     echo "Running unit tests in project $fname"
#     if ! dotnet test $fname; then
#         echo "Error when executing unit tests for $fname"
#         exit 1
#     fi
# done
dotnet $DIR/SonarScanner.MSBuild.dll end /d:sonar.login="$SONAR_KEY"