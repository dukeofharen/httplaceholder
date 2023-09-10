ALTER TABLE stubs
    ADD distribution_key VARCHAR(300);
CREATE INDEX stubs_dist_key ON stubs (distribution_key);

ALTER TABLE requests
    ADD distribution_key VARCHAR(300);
CREATE INDEX requests_dist_key ON requests (distribution_key);

ALTER TABLE responses
    ADD distribution_key VARCHAR(300);
CREATE INDEX responses_dist_key ON responses (distribution_key);
