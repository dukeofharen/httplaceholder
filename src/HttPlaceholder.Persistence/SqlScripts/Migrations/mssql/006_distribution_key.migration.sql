ALTER TABLE stubs
    ADD distribution_key [nvarchar](300);
ALTER TABLE requests
    ADD distribution_key [nvarchar](300);
ALTER TABLE responses
    ADD distribution_key [nvarchar](300);
