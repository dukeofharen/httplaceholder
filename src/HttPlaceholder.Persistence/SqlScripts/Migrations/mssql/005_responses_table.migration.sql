-- Add responses table
IF OBJECT_ID(N'dbo.responses', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[responses](
    [id] [bigint] IDENTITY(1,1) NOT NULL,
    [http_status_code] [int] NOT NULL,
    [headers] [nvarchar](max) NOT NULL,
    [content] [nvarchar](max) NOT NULL,
    [content_is_base64] [tinyint] NOT NULL,
    CONSTRAINT PK_Responses PRIMARY KEY NONCLUSTERED (id),
    CONSTRAINT FK_Responses_Requests FOREIGN KEY (id) REFERENCES requests (id)
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
END;
