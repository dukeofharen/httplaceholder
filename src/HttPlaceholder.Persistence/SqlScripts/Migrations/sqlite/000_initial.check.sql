-- Check whether the stubs table already exists
SELECT COUNT(*)
FROM sqlite_schema AS m
WHERE m.type = 'table'
  AND m.tbl_name = 'stubs';
