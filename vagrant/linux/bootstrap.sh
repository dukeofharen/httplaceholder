#!/bin/bash
SOURCE_PATH="/httplaceholder"
LINUX_VAGRANT_PATH="$SOURCE_PATH/vagrant/linux"
HTTPL_PACKAGE_PATH="$SOURCE_PATH/dist/httplaceholder_linux-x64.tar.gz"
if [ ! -f "$HTTPL_PACKAGE_PATH" ]; then
  echo "Path $HTTPL_PACKAGE_PATH doesn't exist, exiting..."
  exit 1
else
  echo "Path $HTTPL_PACKAGE_PATH exists, continuing."
fi

# Install Nginx
sudo apt update
sudo apt install nginx -y

# Creating HttPlaceholder folder
export INSTALL_PATH=/opt/httplaceholder
if [ -d "$INSTALL_PATH" ]; then
    echo "Deleting path $INSTALL_PATH"
    sudo rm -r $INSTALL_PATH
fi

echo "Creating path $INSTALL_PATH"
sudo mkdir $INSTALL_PATH

# Copy dist to temp folder
TEMP_PATH="/tmp/httplaceholder_linux-x64.tar.gz"
if [ -f "$TEMP_PATH" ]; then
  echo "Removing $TEMP_PATH"
  sudo rm $TEMP_PATH
fi

echo "Copying $HTTPL_PACKAGE_PATH to $TEMP_PATH"
sudo cp $HTTPL_PACKAGE_PATH $TEMP_PATH

# Extract dist contents
sudo tar -xvzf $TEMP_PATH -C $INSTALL_PATH

# Move config to correct location
export DATA_PATH=/etc/httplaceholder
if [ ! -d "$DATA_PATH" ]; then
    echo "Creating path $DATA_PATH"
    sudo mkdir $DATA_PATH
fi

sudo cp $LINUX_VAGRANT_PATH/config.json $DATA_PATH

# Install HttPlaceholder systemd service
envsubst < $LINUX_VAGRANT_PATH/httplaceholder.service > /home/vagrant/httplaceholder.service
sudo cp /home/vagrant/httplaceholder.service /etc/systemd/system/httplaceholder.service 
sudo systemctl enable httplaceholder
sudo systemctl stop httplaceholder
sudo systemctl daemon-reload
sudo systemctl start httplaceholder

# Remove default site from Nginx
if [ -f "/etc/nginx/sites-available/default" ]; then
  sudo rm "/etc/nginx/sites-available/default"
fi

if [ -L "/etc/nginx/sites-enabled/default" ]; then
  sudo rm "/etc/nginx/sites-enabled/default"
fi

# Create self signed certificate for website
if [ ! -f "$DATA_PATH/nginx.key" ]; then
    sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -subj "/C=NL/ST=Drenthe/L=Eelde/O=IT/CN=localhost" -keyout $DATA_PATH/nginx.key -out $DATA_PATH/nginx.crt
fi

# Install HttPlaceholder reverse proxy site in Nginx
sudo cp $LINUX_VAGRANT_PATH/httplaceholder.conf /etc/nginx/sites-available
sudo ln -sf /etc/nginx/sites-available/httplaceholder.conf /etc/nginx/sites-enabled/httplaceholder.conf
sudo systemctl restart nginx