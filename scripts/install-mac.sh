#!/bin/bash
set -e
set -u

# This is added to ensure that the whole script is downloaded before running it.
{
  if ! sudo true; then
    echo "Not running as sudo; can't install HttPlaceholder."
    exit 1
  fi

  echo "Attempting to download and install HttPlaceholder"

  JQ_URL="https://github.com/stedolan/jq/releases/download/jq-1.6/jq-osx-amd64"
  JQ_PATH="/tmp/jq"
  if [ ! -f $JQ_PATH ]; then
    echo "Attempting to download JQ from URL $JQ_URL to $JQ_PATH"
    curl -J -L $JQ_URL --output $JQ_PATH
  fi

  chmod 755 $JQ_PATH

  RELEASES_URL="https://api.github.com/repos/dukeofharen/httplaceholder/releases"
  RELEASES_JSON=$(curl $RELEASES_URL)
  TAG=$(echo "$RELEASES_JSON" | $JQ_PATH -r '.[0] | .tag_name')
  echo "Latest version is $TAG"

  DOWNLOAD_URL=$(echo "$RELEASES_JSON" | eval $JQ_PATH "-r '.[0] | .assets | .[] | select(.name | contains(\"osx\")) | .browser_download_url'")
  echo "Download binaries from $DOWNLOAD_URL"
  TAR_PATH="/tmp/httplaceholder.tar.gz"
  curl -J -L $DOWNLOAD_URL --output $TAR_PATH

  if [ ! -d "/usr/local/bin" ]; then
    echo "Creating directory /usr/local/bin"
    mkdir /usr/local/bin
  fi

  TAR_EXTRACT_LOCATION="/usr/local/bin/httplaceholder-stub"
  echo "Extracting $TAR_PATH to $TAR_EXTRACT_LOCATION"
  if [ -d "$TAR_EXTRACT_LOCATION" ]; then
    echo "Deleting $TAR_EXTRACT_LOCATION"
    sudo rm -R $TAR_EXTRACT_LOCATION
  fi

  echo "Creating $TAR_EXTRACT_LOCATION"
  sudo mkdir $TAR_EXTRACT_LOCATION

  sudo tar -xzf $TAR_PATH -C $TAR_EXTRACT_LOCATION

  LN_PATH="/usr/local/bin/httplaceholder"
  HTTPLACEHOLDER_BIN_PATH="$TAR_EXTRACT_LOCATION/HttPlaceholder"
  echo "Creating symlink from $HTTPLACEHOLDER_BIN_PATH to $LN_PATH"
  sudo ln -sf $HTTPLACEHOLDER_BIN_PATH $LN_PATH

  echo "HttPlaceholder version $TAG is installed. Start application by running 'httplaceholder'."
}
