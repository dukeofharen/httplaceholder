ALTER TABLE stubs
    ADD distribution_key [nvarchar](300) NOT NULL DEFAULT '';
drop index ix_stub_id on stubs;
CREATE INDEX stubs_dist_key ON stubs (distribution_key);
CREATE UNIQUE NONCLUSTERED INDEX [ix_stub_id_distribution_key] ON [dbo].[stubs] (stub_id, distribution_key);

ALTER TABLE requests
    ADD distribution_key [nvarchar](300);
CREATE INDEX requests_dist_key ON requests (distribution_key);

ALTER TABLE responses
    ADD distribution_key [nvarchar](300);
CREATE INDEX responses_dist_key ON responses (distribution_key);
