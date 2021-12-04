#!/bin/bash
sleep 20s
echo "Initializing httplaceholder database"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /usr/local/bin/initscripts/create-db.sql