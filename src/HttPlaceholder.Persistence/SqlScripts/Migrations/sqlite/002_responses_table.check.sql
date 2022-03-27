-- Check whether the responses table already exists
SELECT COUNT(*)
FROM sqlite_schema AS m
WHERE m.type = 'table'
  AND m.tbl_name = 'responses';
