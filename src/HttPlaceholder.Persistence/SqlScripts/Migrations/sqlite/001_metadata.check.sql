-- Check whether the metadata table already exists
SELECT COUNT(*)
FROM sqlite_schema AS m
WHERE m.type = 'table'
  AND m.tbl_name = 'metadata';
