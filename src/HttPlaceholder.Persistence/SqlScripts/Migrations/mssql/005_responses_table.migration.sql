-- Add responses table
IF OBJECT_ID(N'dbo.responses', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[responses]
        (
            [id]             [bigint] UNIQUE NOT NULL,
            [status_code]    [int]           NOT NULL,
            [headers]        [nvarchar](max) NOT NULL,
            [body]           [nvarchar](max) NOT NULL,
            [body_is_binary] [tinyint]       NOT NULL,
            CONSTRAINT PK_Responses PRIMARY KEY NONCLUSTERED (id),
            CONSTRAINT FK_Responses_Requests FOREIGN KEY (id) REFERENCES requests (id) ON DELETE CASCADE
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
    END;
