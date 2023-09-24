SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE (table_name = 'scenarios')
  AND (table_schema = DATABASE())
