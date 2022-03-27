SELECT COUNT(*)
FROM sys.indexes
WHERE name = 'PK_Stubs'
  AND object_id = OBJECT_ID('dbo.stubs')
