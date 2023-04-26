#/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PFX_PATH="$DIR/key.pfx"
CRT_PATH="/tmp/httplaceholder.crt"

openssl pkcs12 -in "$PFX_PATH" -clcerts -nokeys -out "$CRT_PATH" -password pass:1234
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain "$CRT_PATH"