SELECT COUNT(*)
FROM sqlite_schema AS m,
     pragma_table_info(m.name) AS ti
WHERE m.type = 'table'
  AND m.tbl_name = 'stubs'
  AND ti.name = 'distribution_key';
