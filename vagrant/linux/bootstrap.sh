#!/bin/bash

# Install Nginx
apt update
apt install nginx -y

# Install .NET Core
apt update
apt install -y dotnet6

# Install PIP for Python
apt install -y python3-pip

# Install Node.js
NODE_BIN_PATH=/bin/node
if [ ! -f "$NODE_BIN_PATH" ]; then
    echo "Node not installled; installing now."
    wget https://nodejs.org/dist/v16.14.0/node-v16.14.0-linux-x64.tar.xz -O node.tar.xz
    NODE_EXTRACT_PATH=/opt/node
    if [ -d "$NODE_EXTRACT_PATH" ]; then
        echo "Deleting path $NODE_EXTRACT_PATH"
        rm -r $NODE_EXTRACT_PATH
    fi
    
    mkdir $NODE_EXTRACT_PATH
    tar -xf node.tar.xz -C $NODE_EXTRACT_PATH
    ln -sf /opt/node/node-v16.14.0-linux-x64/bin/node /bin/node
    ln -sf /opt/node/node-v16.14.0-linux-x64/lib/node_modules/npm/bin/npm-cli.js /bin/npm
    ln -sf /opt/node/node-v16.14.0-linux-x64/lib/node_modules/npm/bin/npx-cli.js /bin/npx
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
rsync -av --progress /httplaceholder/. $NEW_SOURCE_PATH --exclude bin --exclude obj --exclude node_modules --exclude TestResults --exclude .git

# Build HttPlaceholder
PROJECT_PATH=$NEW_SOURCE_PATH/src/HttPlaceholder/HttPlaceholder.csproj
dotnet publish $PROJECT_PATH -c Release --runtime linux-x64 -o $INSTALL_PATH

# Build HttPlaceholder GUI
GUI_PROJECT_PATH=$NEW_SOURCE_PATH/gui
cd "$GUI_PROJECT_PATH"
if [ -d "dist" ]; then
  echo "Deleting folder dist"
  rm -r "dist"
fi

npm install
npm run build

GUI_PATH=$INSTALL_PATH/gui
if [ -d "$GUI_PATH" ]; then
  echo "Re-creating folder $GUI_PATH"
  rm -r "$GUI_PATH"
  mkdir "$GUI_PATH"  
fi

cp -r dist/. "$GUI_PATH"

# Build docs for use in UI
DOCS_SOURCE_DIR="$NEW_SOURCE_PATH/docs/httpl-docs"
GUI_DOCS_DIR="$GUI_PATH/docs"
if [ -d "$GUI_DOCS_DIR" ]; then
  echo "Re-creating folder $GUI_DOCS_DIR"
  rm -r "$GUI_DOCS_DIR"
  mkdir "$GUI_DOCS_DIR"
fi

echo "Building documentation"
cd "$DOCS_SOURCE_DIR"
pip install mkdocs
python3 sync.py
mkdocs build
cp -r site/. "$GUI_DOCS_DIR"

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
if [ ! -f "$DATA_PATH/nginx.key" ]; then
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -subj "/C=NL/ST=Drenthe/L=Eelde/O=IT/CN=localhost" -keyout $DATA_PATH/nginx.key -out $DATA_PATH/nginx.crt
fi

# Install HttPlaceholder reverse proxy site in Nginx
sudo cp $NEW_SOURCE_PATH/vagrant/linux/httplaceholder.conf /etc/nginx/sites-available
ln -sf /etc/nginx/sites-available/httplaceholder.conf /etc/nginx/sites-enabled/httplaceholder.conf
systemctl restart nginx