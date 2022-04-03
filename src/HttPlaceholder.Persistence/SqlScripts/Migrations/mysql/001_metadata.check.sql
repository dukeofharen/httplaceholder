SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE (table_name = 'metadata')
  AND (table_schema = DATABASE())
