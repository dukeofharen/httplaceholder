-- Create metadata table
CREATE TABLE IF NOT EXISTS `metadata`
(
    `id`                      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `stub_update_tracking_id` TEXT    NOT NULL UNIQUE
);

-- Insert initial data in the metadata table
INSERT INTO `metadata` (stub_update_tracking_id)
SELECT 'initial_value'
    WHERE NOT EXISTS(SELECT * FROM metadata);
