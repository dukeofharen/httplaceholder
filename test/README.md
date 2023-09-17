# Tests

The integration tests use Hurl (https://hurl.dev/). To run the tests, make sure to have `hurl` on your path and to have Docker (Compose) installed. When executing the script `exec-tests.sh`, HttPlaceholder will be executed in several configurations (e.g. in memory, MySQL, file storage etc.) and all the Hurl tests will be executed against HttPlaceholder.