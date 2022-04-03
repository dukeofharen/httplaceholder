-- Create requests table
CREATE TABLE IF NOT EXISTS `requests`
(
    `id`                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `correlation_id`     TEXT    NOT NULL,
    `executing_stub_id`  TEXT,
    `request_begin_time` TEXT    NOT NULL,
    `request_end_time`   TEXT,
    `json`               TEXT    NOT NULL
);

-- Create stubs table
CREATE TABLE IF NOT EXISTS `stubs`
(
    `id`        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `stub_id`   TEXT    NOT NULL UNIQUE,
    `stub`      TEXT    NOT NULL,
    `stub_type` TEXT    NOT NULL
);

