#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PFX_PATH="$DIR/key.pfx"
CRT_PATH="/tmp/httplaceholder.crt"
if [[ ! -f "$PFX_PATH" ]]; then
    echo "Path $PFX_PATH not found"
    exit 1
fi

openssl pkcs12 -in "$PFX_PATH" -clcerts -nokeys -out "$CRT_PATH" -password pass:1234
sudo mv "$CRT_PATH" "/usr/local/share/ca-certificates"

# TODO this command is for Debian; also add commands for other distros.
sudo update-ca-certificates