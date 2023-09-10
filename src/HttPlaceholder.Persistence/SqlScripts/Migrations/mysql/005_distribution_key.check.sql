SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE (table_name = 'stubs')
  AND (table_schema = DATABASE())
  AND (column_name = 'distribution_key')
