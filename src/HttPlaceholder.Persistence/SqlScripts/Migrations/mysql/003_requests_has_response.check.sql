SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE (table_name = 'requests')
  AND (table_schema = DATABASE())
  AND (column_name = 'has_response')
