SELECT COUNT(*)
FROM pg_tables
WHERE schemaname = "current_schema"()
  AND tablename = 'stubs';
