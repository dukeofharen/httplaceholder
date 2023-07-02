#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PFX_PATH="$DIR/../../src/HttPlaceholder/key.pfx"
if [ ! -f "$PFX_PATH" ]; then
  echo "Path $PFX_PATH not found."
  exit 1
fi

METADATA_PATH="$DIR/.metadata"
if [ ! -d "$METADATA_PATH" ]; then
  echo "Creating directory $METADATA_PATH"
  mkdir $METADATA_PATH  
fi

PRIVATE_KEY_PATH="$METADATA_PATH/httplaceholder.key"
openssl genrsa -out "$PRIVATE_KEY_PATH"

CSR_PATH="$METADATA_PATH/httplaceholder.csr"
openssl req -key "$PRIVATE_KEY_PATH" -new -out "$CSR_PATH" -subj "/C=NL/ST=Drenthe/L=Eelde/O=HttPlaceholder/OU=HttPlaceholder/CN=localhost"

CRT_PATH="$METADATA_PATH/httplaceholder.crt"
openssl x509 -signkey "$PRIVATE_KEY_PATH" -in "$CSR_PATH" -req -days 730 -out "$CRT_PATH"

openssl pkcs12 -password pass:1234 -inkey "$PRIVATE_KEY_PATH" -in "$CRT_PATH" -export -out "$PFX_PATH"