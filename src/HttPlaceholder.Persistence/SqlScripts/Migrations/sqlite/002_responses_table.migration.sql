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
