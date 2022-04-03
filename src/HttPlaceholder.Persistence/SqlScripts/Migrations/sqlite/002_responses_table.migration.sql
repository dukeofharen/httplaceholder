-- Create responses table
CREATE TABLE IF NOT EXISTS `responses`
(
    `id`             INTEGER NOT NULL PRIMARY KEY,
    `status_code`    INT     NOT NULL,
    `headers`        TEXT    NOT NULL,
    `body`           TEXT    NOT NULL,
    `body_is_binary` INT     NOT NULL,
    FOREIGN KEY (id) REFERENCES requests (id) ON DELETE CASCADE
);
