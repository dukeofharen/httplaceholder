#!/bin/bash

# Install .NET Core SDK
# TODO check if dotnet is already installed
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
add-apt-repository universe
apt-get install -y apt-transport-https
apt-get update
apt-get install -y dotnet-sdk-2.2

# # Watch HttPlaceholder changes
# cd /vagrant/src/HttPlaceholder
# dotnet watch run