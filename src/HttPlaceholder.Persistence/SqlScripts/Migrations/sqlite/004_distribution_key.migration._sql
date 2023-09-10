CREATE TABLE `tmp_stubs`
(
    `id`               INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `stub_id`          TEXT    NOT NULL,
    `stub`             TEXT    NOT NULL,
    `stub_type`        TEXT    NOT NULL,
    `distribution_key` TEXT    NOT NULL
);
INSERT INTO tmp_stubs (id, stub_id, stub, stub_type, distribution_key)
SELECT id, stub_id, stub, stub_type, ''
FROM stubs;
DROP TABLE stubs;
ALTER TABLE tmp_stubs RENAME TO stubs;
CREATE UNIQUE INDEX IF NOT EXISTS stubs_stub_id_dist_key ON stubs (stub_id, distribution_key);

ALTER TABLE requests
    ADD distribution_key TEXT;
CREATE INDEX requests_dist_key ON requests (distribution_key);

ALTER TABLE responses
    ADD distribution_key TEXT;
CREATE INDEX responses_dist_key ON responses (distribution_key);
