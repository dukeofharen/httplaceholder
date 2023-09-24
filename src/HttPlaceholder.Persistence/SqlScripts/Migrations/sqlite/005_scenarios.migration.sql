-- Create scenarios table
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`               INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `distribution_key` TEXT    NOT NULL DEFAULT '',
    `scenario`         TEXT    NOT NULL UNIQUE,
    `state`            TEXT    NOT NULL,
    `hit_count`        INT     NOT NULL
);
CREATE INDEX `ix_distribution_key` ON scenarios (distribution_key);
