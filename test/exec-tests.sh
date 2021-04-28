#!/bin/bash
# Execute this script for performing Postman integration tests against HttPlaceholder.
# You need to have Newman installed (https://github.com/postmanlabs/newman).
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
HTTPL_ROOT_DIR=$DIR/../src/HttPlaceholder
POSTMAN_PATH=$DIR/HttPlaceholderIntegration.postman_collection.json

# Run HttPlaceholder tests for in memory configuration.
echo "Testing HttPlaceholder with in memory configuration"
dotnet run -p $HTTPL_ROOT_DIR --useInMemoryStorage > $DIR/logs.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in file storage configuration.
echo "Testing HttPlaceholder with in file storage configuration"
FILE_STORAGE_PATH="/tmp/httplaceholder_stubs"
if [ ! -d "$FILE_STORAGE_PATH" ]; then
  echo "Creating folder $FILE_STORAGE_PATH"
  mkdir $FILE_STORAGE_PATH
fi

dotnet run -p $HTTPL_ROOT_DIR --fileStorageLocation $FILE_STORAGE_PATH > $DIR/logs.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in SQLite configuration.
echo "Testing HttPlaceholder with in SQLite configuration"
SQLITE_PATH="/tmp/httplaceholder_stubs.db"
sudo rm $SQLITE_PATH
dotnet run -p $HTTPL_ROOT_DIR --sqliteConnectionString "Data Source=$SQLITE_PATH" > $DIR/logs.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure
sudo killall HttPlaceholder