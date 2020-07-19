#!/bin/bash

# Install Nginx
apt update
apt install nginx -y

# Install .NET Core
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt update
apt install -y apt-transport-https
apt update
apt install -y dotnet-sdk-3.1

# Install Node.js
NODE_BIN_PATH=/bin/node
if [ ! -f "$NODE_BIN_PATH" ]; then
    echo "Node not installled; installing now."
    wget https://nodejs.org/dist/v12.18.2/node-v12.18.2-linux-x64.tar.xz -O node-12.18.2.tar.xz
    NODE_EXTRACT_PATH=/opt/node
    if [ -d "$NODE_EXTRACT_PATH" ]; then
        echo "Deleting path $NODE_EXTRACT_PATH"
        rm -r $NODE_EXTRACT_PATH
    fi
    
    mkdir $NODE_EXTRACT_PATH
    tar -xf node-12.18.2.tar.xz -C $NODE_EXTRACT_PATH
    ln -sf /opt/node/node-v12.18.2-linux-x64/bin/node /bin/node
    ln -sf /opt/node/node-v12.18.2-linux-x64/lib/node_modules/npm/bin/npm-cli.js /bin/npm
    ln -sf /opt/node/node-v12.18.2-linux-x64/lib/node_modules/npm/bin/npx-cli.js /bin/npx
fi

# Creating HttPlaceholder folder
export INSTALL_PATH=/opt/httplaceholder
if [ -d "$INSTALL_PATH" ]; then
    echo "Deleting path $INSTALL_PATH"
    rm -r $INSTALL_PATH
fi

echo "Creating path $INSTALL_PATH"
mkdir $INSTALL_PATH

# Copy source to user home folder
NEW_SOURCE_PATH=/home/vagrant/httplaceholder
if [ -d "$NEW_SOURCE_PATH" ]; then
    echo "Deleting path $NEW_SOURCE_PATH"
    rm -r $NEW_SOURCE_PATH
fi

mkdir $NEW_SOURCE_PATH
echo "Copying source code to $NEW_SOURCE_PATH"
rsync -av --progress /httplaceholder/. $NEW_SOURCE_PATH --exclude bin --exclude obj --exclude node_modules

# Build HttPlaceholder
PROJECT_PATH=$NEW_SOURCE_PATH/src/HttPlaceholder/HttPlaceholder.csproj
dotnet publish $PROJECT_PATH -c Release --runtime linux-x64 -o $INSTALL_PATH

# Build HttPlaceholder GUI
GUI_PROJECT_PATH=$NEW_SOURCE_PATH/gui
cd $GUI_PROJECT_PATH
npm install
npm run build
cp -r dist/. $INSTALL_PATH/gui

# Move config to correct location
export DATA_PATH=/etc/httplaceholder
if [ ! -d "$DATA_PATH" ]; then
    echo "Creating path $DATA_PATH"
    mkdir $DATA_PATH
fi

cp $NEW_SOURCE_PATH/vagrant/linux/config.json $DATA_PATH

# Install HttPlaceholder systemd service
envsubst < $NEW_SOURCE_PATH/vagrant/linux/httplaceholder.service > /etc/systemd/system/httplaceholder.service
sudo systemctl enable httplaceholder
sudo systemctl stop httplaceholder
sudo systemctl daemon-reload
sudo systemctl start httplaceholder

# Remove default site from Nginx
sudo rm /etc/nginx/sites-available/default
sudo rm /etc/nginx/sites-enabled/default

# Create self signed certificate for website
if [ ! -f "/etc/httplaceholder/nginx.key" ]; then
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -subj "/C=NL/ST=Drenthe/L=Eelde/O=IT/CN=localhost" -keyout /etc/httplaceholder/nginx.key -out /etc/httplaceholder/nginx.crt
fi

# Install HttPlaceholder reverse proxy site in Nginx
sudo cp $NEW_SOURCE_PATH/vagrant/linux/httplaceholder.conf /etc/nginx/sites-available
ln -sf /etc/nginx/sites-available/httplaceholder.conf /etc/nginx/sites-enabled/httplaceholder.conf
systemctl restart nginx