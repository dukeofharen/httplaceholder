-- Add scenarios table
IF OBJECT_ID(N'dbo.scenarios', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[scenarios]
        (
            [id]        [bigint] IDENTITY (1,1) NOT NULL,
            [scenario]  [nvarchar](max)         NOT NULL,
            [state]     [nvarchar](max)         NOT NULL,
            [hit_count] [int]                   NOT NULL
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
    END;
