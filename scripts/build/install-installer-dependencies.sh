#!/bin/bash
# Install zip
apt update
apt install zip -y

# Install NSIS
DOWNLOAD_URL="https://deac-ams.dl.sourceforge.net/project/nsis/NSIS%202/2.51/nsis-2.51.zip"
wget $DOWNLOAD_URL
unzip nsis-2.51.zip
wine nsis-2.51/makensis.exe