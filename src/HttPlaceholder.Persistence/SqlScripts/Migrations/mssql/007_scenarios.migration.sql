-- Add scenarios table
IF OBJECT_ID(N'dbo.scenarios', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[scenarios]
        (
            [id]               [bigint] IDENTITY (1,1) NOT NULL,
            [distribution_key] [nvarchar](300)         NOT NULL DEFAULT '',
            [scenario]         [nvarchar](500)         NOT NULL,
            [state]            [nvarchar](500)         NOT NULL,
            [hit_count]        [int]                   NOT NULL
        ) ON [PRIMARY];
    END;
CREATE UNIQUE NONCLUSTERED INDEX [ix_scenario] ON [dbo].[scenarios] (scenario);
CREATE UNIQUE NONCLUSTERED INDEX [ix_distribution_key] ON [dbo].[scenarios] (distribution_key);
