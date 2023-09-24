-- Create scenarios table
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `scenario`    TEXT    NOT NULL,
    `state`       TEXT    NOT NULL,
    `hit_count`   INT     NOT NULL
);
