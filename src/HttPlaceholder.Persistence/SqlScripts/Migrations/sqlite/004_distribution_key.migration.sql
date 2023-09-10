ALTER TABLE stubs ADD has_response INT;

ALTER TABLE stubs
    ADD distribution_key TEXT;
CREATE INDEX stubs_dist_key ON stubs (distribution_key);

ALTER TABLE requests
    ADD distribution_key TEXT;
CREATE INDEX requests_dist_key ON requests (distribution_key);

ALTER TABLE responses
    ADD distribution_key TEXT;
CREATE INDEX responses_dist_key ON responses (distribution_key);
