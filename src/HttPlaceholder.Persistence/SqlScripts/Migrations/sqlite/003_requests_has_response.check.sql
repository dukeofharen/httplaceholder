-- Check whether the "has_response" field already exists
SELECT COUNT(*)
FROM sqlite_schema AS m,
     pragma_table_info(m.name) AS ti
WHERE m.type = 'table'
  AND m.tbl_name = 'requests'
  AND ti.name = 'has_response';
