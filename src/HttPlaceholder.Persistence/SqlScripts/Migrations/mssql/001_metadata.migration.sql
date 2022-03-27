-- Create metadata table and indices
IF OBJECT_ID(N'dbo.metadata', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[metadata](
    [id] [bigint] IDENTITY(1,1) NOT NULL,
    [stub_update_tracking_id] [nvarchar](50)
    ) ON [PRIMARY];
END;

-- Insert initial data in the metadata table
IF (SELECT COUNT(*) FROM [dbo].[metadata]) = 0
BEGIN
INSERT INTO [dbo].[metadata] (stub_update_tracking_id) VALUES ('initial_value');
END;
