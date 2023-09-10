ALTER TABLE stubs
    ADD distribution_key VARCHAR(300) NOT NULL DEFAULT '';
DROP INDEX ix_stub_id ON stubs;
CREATE INDEX stubs_dist_key ON stubs (distribution_key);
CREATE UNIQUE INDEX stubs_stub_id_dist_key ON stubs (stub_id, distribution_key);

ALTER TABLE requests
    ADD distribution_key VARCHAR(300);
CREATE INDEX requests_dist_key ON requests (distribution_key);

ALTER TABLE responses
    ADD distribution_key VARCHAR(300);
CREATE INDEX responses_dist_key ON responses (distribution_key);
