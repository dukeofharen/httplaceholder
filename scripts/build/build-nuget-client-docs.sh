#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
ROOT_DIR=$DIR/../..
CLIENT_DIR=$ROOT_DIR/src/HttPlaceholder.Client
DIST_PATH=$ROOT_DIR/dist
if [ ! -d "$ROOT_DIR" ]; then
  mkdir $DIST_PATH  
fi

cd $CLIENT_DIR

# Install cURL
apt update -y
apt install curl -y

# Download Doxygen
DOXYGEN_FILE="doxygen.tar.gz"
DOXYGEN_EXTRACT_PATH="/tmp/doxygen"
if [ ! -f $DOXYGEN_FILE ]; then
  curl -o $DOXYGEN_FILE https://www.doxygen.nl/files/doxygen-1.9.3.linux.bin.tar.gz
fi

if [ -d $DOXYGEN_EXTRACT_PATH ]; then
  rm -r $DOXYGEN_EXTRACT_PATH
fi

mkdir $DOXYGEN_EXTRACT_PATH
tar -xvzf $DOXYGEN_FILE -C $DOXYGEN_EXTRACT_PATH

# Create docs
$DOXYGEN_EXTRACT_PATH/doxygen-1.9.3/bin/doxygen

# Pack docks
cd $CLIENT_DIR/docs/html
tar -zcvf $DIST_PATH/nuget-client-docs.tar.gz *