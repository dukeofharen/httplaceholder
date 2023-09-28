-- Create scenarios table
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`               INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `distribution_key` TEXT    NOT NULL DEFAULT '',
    `scenario`         TEXT    NOT NULL,
    `state`            TEXT    NOT NULL,
    `hit_count`        INT     NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ix_scenario_distribution_key ON scenarios (scenario, distribution_key);
