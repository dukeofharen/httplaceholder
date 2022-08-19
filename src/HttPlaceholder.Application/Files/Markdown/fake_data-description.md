The fake data makes it possible to insert random data to the response. The fake data is generated using [Bogus](https://github.com/bchavez/Bogus). You can insert the following data:

- Address: zipcode, city, street_address, city_prefix, city_suffix, street_name, building_number, street_suffix, secondary_address, county, country, full_address, country_code, state, state_abbreviation, direction, cardinal_direction, ordinal_direction
- Name: first_name, last_name, full_name, prefix, suffix, job_title, job_descriptor, job_area, job_type
- Phone: phone_number
- Internet: email, example_email, user_name, user_name_unicode, domain_name, domain_word, domain_suffix, ip, port, ipv6, user_agent, mac, password, color, protocol, url, url_with_path, url_rooted_path
- Lorem: word, words, letter, sentence, sentences, paragraph, paragraphs, text, lines, slug
- Date: past, past_offset, soon, soon_offset, future, future_offset, recent, recent_offset, month, weekday,  timezone_string
- Finance: account, account_name, amount, currency_name, currency_code, currency_symbol, credit_card_number, credit_card_cvv, routing_number, bic, iban, bitcoin_address, ethereum_address, litecoin_address
- System: file_name, directory_path, file_path, common_file_name, mime_type, common_file_type, common_file_ext, file_type, file_ext, semver, android_id, apple_push_token
- Commerce: department, price, product_name, product, product_adjective, product_description, ean8, ean13

The following locales are supported: [LOCALES].

This handler can be inserted in the following ways:

- `((fake_data:first_name))` (only specify the generator).
- `((fake_data:en_US:first_name))` (specify generator and locale).
- `((fake_data:past:yyyy-MM-dd HH:mm:ss))` (specify generator and formatting string, if applicable).
- `((fake_data:en_US:past:yyyy-MM-dd HH:mm:ss))` (specify generator, locale and formatting string, if applicable).
