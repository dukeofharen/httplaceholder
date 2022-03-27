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

-- Create responses table
CREATE TABLE IF NOT EXISTS `responses`
(
    `id`                INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `http_status_code`  INT     NOT NULL,
    `headers`           TEXT    NOT NULL,
    `content`           TEXT    NOT NULL,
    `content_is_base64` INT     NOT NULL,
    FOREIGN KEY (id) REFERENCES requests (id)
    );
