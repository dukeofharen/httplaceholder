-- Create stubs table and indices
IF OBJECT_ID(N'dbo.stubs', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[stubs]
        (
            [id]        [bigint] IDENTITY (1,1) NOT NULL,
            [stub_id]   [nvarchar](255)         NOT NULL,
            [stub]      [nvarchar](max)         NOT NULL,
            [stub_type] [nvarchar](20)          NOT NULL
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
    END;

IF (SELECT COUNT(*)
    FROM sys.indexes
    WHERE name = 'ix_stub_id'
      AND object_id = OBJECT_ID('dbo.stubs')) = 0
CREATE UNIQUE NONCLUSTERED INDEX [ix_stub_id] ON [dbo].[stubs]
    (
     [stub_id] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

-- Create requests table and indices
IF OBJECT_ID(N'dbo.requests', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[requests]
        (
            [id]                 [bigint] IDENTITY (1,1) NOT NULL,
            [correlation_id]     [nvarchar](100)         NOT NULL,
            [executing_stub_id]  [nvarchar](100)         NULL,
            [request_begin_time] [datetime2](7)          NOT NULL,
            [request_end_time]   [datetime2](7)          NULL,
            [json]               [nvarchar](max)         NOT NULL
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
    END;

IF (SELECT COUNT(*)
    FROM sys.indexes
    WHERE name = 'ix_correlation_id'
      AND object_id = OBJECT_ID('dbo.requests')) = 0
CREATE UNIQUE NONCLUSTERED INDEX [ix_correlation_id] ON [dbo].[requests]
    (
     [correlation_id] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
