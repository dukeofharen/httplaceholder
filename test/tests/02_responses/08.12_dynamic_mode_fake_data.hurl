# Create stub
POST {{rootUrl}}/ph-api/stubs
Content-Type: text/yaml
```
id: dynamic-mode-fake-data
tenant: integration
conditions:
  method: GET
  url:
    path:
      equals: /dynamic-mode-fake-data
response:
  enableDynamicMode: true
  contentType: text/plain;charset=utf-8
  text: |
    Examples with locale:
    first_name: ((fake_data:pt_BR:first_name))
    soon: ((fake_data:tr:soon:yyyy MMMM dd))
    Address:
    zipcode: ((fake_data:zipcode))
    city: ((fake_data:city))
    street_address: ((fake_data:street_address))
    city_prefix: ((fake_data:city_prefix))
    city_suffix: ((fake_data:city_suffix))
    street_name: ((fake_data:street_name))
    building_number: ((fake_data:building_number))
    street_suffix: ((fake_data:street_suffix))
    secondary_address: ((fake_data:secondary_address))
    county: ((fake_data:county))
    country: ((fake_data:country))
    full_address: ((fake_data:full_address))
    country_code: ((fake_data:country_code))
    state: ((fake_data:state))
    state_abbreviation: ((fake_data:state_abbreviation))
    direction: ((fake_data:direction))
    cardinal_direction: ((fake_data:cardinal_direction))
    ordinal_direction: ((fake_data:ordinal_direction))
    Name:
    first_name: ((fake_data:first_name))
    last_name: ((fake_data:last_name))
    full_name: ((fake_data:full_name))
    prefix: ((fake_data:prefix))
    suffix: ((fake_data:suffix))
    job_title: ((fake_data:job_title))
    job_descriptor: ((fake_data:job_descriptor))
    job_area: ((fake_data:job_area))
    job_type: ((fake_data:job_type))
    Phone:
    phone_number: ((fake_data:phone_number))
    Internet:
    email: ((fake_data:email))
    example_email: ((fake_data:example_email))
    user_name: ((fake_data:user_name))
    user_name_unicode: ((fake_data:user_name_unicode))
    domain_name: ((fake_data:domain_name))
    domain_word: ((fake_data:domain_word))
    domain_suffix: ((fake_data:domain_suffix))
    ip: ((fake_data:ip))
    port: ((fake_data:port))
    ipv6: ((fake_data:ipv6))
    user_agent: ((fake_data:user_agent))
    mac: ((fake_data:mac))
    password: ((fake_data:password))
    color: ((fake_data:color))
    protocol: ((fake_data:protocol))
    url: ((fake_data:url))
    url_with_path: ((fake_data:url_with_path))
    url_rooted_path: ((fake_data:url_rooted_path))
    Lorem:
    word: ((fake_data:word))
    words: ((fake_data:words))
    words with formatting: ((fake_data:words:10))
    letter: ((fake_data:letter))
    letter with formatting: ((fake_data:letter:30))
    sentence: ((fake_data:sentence))
    sentence with formatting: ((fake_data:sentence:20))
    sentences: ((fake_data:sentences))
    sentences with formatting: ((fake_data:sentences:10))
    paragraph: ((fake_data:paragraph))
    paragraph with formatting: ((fake_data:paragraph:10))
    paragraphs: ((fake_data:paragraphs))
    paragraphs with formatting: ((fake_data:paragraphs:8))
    text: ((fake_data:text))
    lines: ((fake_data:lines))
    lines with formatting: ((fake_data:lines:10))
    slug: ((fake_data:slug))
    slug with formatting: ((fake_data:slug:10))
    Date:
    past: ((fake_data:past))
    past with formatting: ((fake_data:past:yyyy-MM-dd))
    past_offset: ((fake_data:past_offset))
    past_offset with formatting: ((fake_data:past_offset:yyyy-MM-dd))
    soon: ((fake_data:soon))
    soon with formatting: ((fake_data:soon:yyyy-MM-dd))
    soon_offset: ((fake_data:soon_offset))
    soon_offset with formatting: ((fake_data:soon_offset:yyyy-MM-dd))
    future: ((fake_data:future))
    future_with_formatting: ((fake_data:future:yyyy-MM-dd))
    future_offset: ((fake_data:future_offset))
    future_offset with formatting: ((fake_data:future_offset:yyyy-MM-dd))
    recent: ((fake_data:recent))
    recent with formatting: ((fake_data:recent:yyyy-MM-dd))
    recent_offset: ((fake_data:recent_offset))
    recent_offset with formatting: ((fake_data:recent_offset:yyyy-MM-dd))
    month: ((fake_data:month))
    weekday: ((fake_data:weekday))
    timezone_string: ((fake_data:timezone_string))
    Finance:
    account: ((fake_data:account))
    account_name: ((fake_data:account_name))
    amount: ((fake_data:amount))
    amount with formatting: ((fake_data:amount:0.000))
    currency_name: ((fake_data:currency_name))
    currency_code: ((fake_data:currency_code))
    credit_card_number: ((fake_data:credit_card_number))
    credit_card_cvv: ((fake_data:credit_card_cvv))
    routing_number: ((fake_data:routing_number))
    bic: ((fake_data:bic))
    iban: ((fake_data:iban))
    bitcoin_address: ((fake_data:bitcoin_address))
    ethereum_address: ((fake_data:ethereum_address))
    litecoin_address: ((fake_data:litecoin_address))
    System:
    file_name: ((fake_data:file_name))
    directory_path: ((fake_data:directory_path))
    file_path: ((fake_data:file_path))
    common_file_name: ((fake_data:common_file_name))
    mime_type: ((fake_data:mime_type))
    common_file_type: ((fake_data:common_file_type))
    common_file_ext: ((fake_data:common_file_ext))
    file_type: ((fake_data:file_type))
    file_ext: ((fake_data:file_ext))
    semver: ((fake_data:semver))
    android_id: ((fake_data:android_id))
    apple_push_token: ((fake_data:apple_push_token))
    Commerce:
    department: ((fake_data:department))
    price: ((fake_data:price))
    product_name: ((fake_data:product_name))
    product: ((fake_data:product))
    product_adjective: ((fake_data:product_adjective))
    product_description: ((fake_data:product_description))
    ean8: ((fake_data:ean8))
    ean13: ((fake_data:ean13))
priority: 0
```

HTTP 200

# Check the request
GET {{rootUrl}}/dynamic-mode-fake-data

HTTP 200
[Asserts]
body contains "first_name"
body not contains "(("