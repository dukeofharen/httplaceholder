#!/bin/bash
# Execute this script for performing Postman integration tests against HttPlaceholder.
# You need to have Newman installed (https://github.com/postmanlabs/newman).
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
HTTPL_ROOT_DIR="$DIR/../src/HttPlaceholder"
POSTMAN_PATH="$DIR/HttPlaceholderIntegration.postman_collection.json"
LOGS_DIR="$DIR/logs"
if [ ! -d "$LOGS_DIR" ]; then
  mkdir "$LOGS_DIR"
fi

function assert-test-ok() {
  if [ $? != "0" ]; then
    echo "Test executed with errors."
    exit 1
  fi
}

# Starting up database containers.
DEVENV_SCRIPT_PATH="$DIR/../scripts/dev/docker-compose.yml"
sudo docker-compose -f "$DEVENV_SCRIPT_PATH" up -d

# Run HttPlaceholder tests for in memory configuration.
echo "Testing HttPlaceholder with in memory configuration"
dotnet run -p $HTTPL_ROOT_DIR --useInMemoryStorage > $DIR/logs/httplaceholder-in-memory.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure > $DIR/logs/test-in-memory.txt
assert-test-ok
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in file storage configuration.
echo "Testing HttPlaceholder with in file storage configuration"
FILE_STORAGE_PATH="/tmp/httplaceholder_stubs"
if [ ! -d "$FILE_STORAGE_PATH" ]; then
  echo "Creating folder $FILE_STORAGE_PATH"
  mkdir $FILE_STORAGE_PATH
fi

dotnet run -p $HTTPL_ROOT_DIR --fileStorageLocation $FILE_STORAGE_PATH > $DIR/logs/httplaceholder-file-storage.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure > $DIR/logs/test-file-storage.txt
assert-test-ok
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in SQLite configuration.
echo "Testing HttPlaceholder with in SQLite configuration"
SQLITE_PATH="/tmp/httplaceholder_stubs.db"
if [ -f "$SQLITE_PATH" ]; then
  sudo rm $SQLITE_PATH
fi

dotnet run -p $HTTPL_ROOT_DIR --sqliteConnectionString "Data Source=$SQLITE_PATH" > $DIR/logs/httplaceholder-sqlite.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure > $DIR/logs/test-sqlite.txt
assert-test-ok
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in MySQL configuration.
echo "Testing HttPlaceholder with in MySQL configuration"
dotnet run -p $HTTPL_ROOT_DIR --mysqlConnectionString "Server=localhost;Database=httplaceholder;Uid=root;Pwd=root;Allow User Variables=true" > $DIR/logs/httplaceholder-mysql.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure > $DIR/logs/test-mysql.txt
assert-test-ok
sudo killall HttPlaceholder

# Run HttPlaceholder tests for in MSSQL configuration.
echo "Testing HttPlaceholder with in MSSQL configuration"
dotnet run -p $HTTPL_ROOT_DIR --sqlServerConnectionString "Server=localhost,1433;Database=httplaceholder;User Id=sa;Password=Password123!" > $DIR/logs/httplaceholder-mssql.txt 2>&1 &
sleep 5
newman run $POSTMAN_PATH --insecure > $DIR/logs/test-mssql.txt
assert-test-ok
sudo killall HttPlaceholder

# Stop the Docker containers.
sudo docker-compose -f "$DEVENV_SCRIPT_PATH" down