SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE (table_name = 'responses')
  AND (table_schema = DATABASE())
