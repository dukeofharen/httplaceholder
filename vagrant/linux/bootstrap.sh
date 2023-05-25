#!/bin/bash
# Install Nginx
sudo apt update
sudo apt install nginx -y

# Install .NET Core
sudo apt update
sudo apt install -y dotnet-sdk-7.0

# Install PIP for Python
sudo apt install -y python3-pip

# Install Node.js
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.3/install.sh | bash
. "$HOME/.nvm/nvm.sh"
nvm install --lts
nvm use --lts

# Creating HttPlaceholder folder
export INSTALL_PATH=/opt/httplaceholder
if [ -d "$INSTALL_PATH" ]; then
    echo "Deleting path $INSTALL_PATH"
    sudo rm -r $INSTALL_PATH
fi

echo "Creating path $INSTALL_PATH"
sudo mkdir $INSTALL_PATH

# Copy source to user home folder
NEW_SOURCE_PATH=/home/vagrant/httplaceholder
if [ -d "$NEW_SOURCE_PATH" ]; then
    echo "Deleting path $NEW_SOURCE_PATH"
    sudo rm -r $NEW_SOURCE_PATH
fi

mkdir $NEW_SOURCE_PATH
echo "Copying source code to $NEW_SOURCE_PATH"
rsync -av --progress /httplaceholder/. $NEW_SOURCE_PATH --exclude bin --exclude obj --exclude node_modules --exclude TestResults --exclude .git

# Build HttPlaceholder
PROJECT_PATH=$NEW_SOURCE_PATH/src/HttPlaceholder/HttPlaceholder.csproj
sudo dotnet publish $PROJECT_PATH -c Release --runtime linux-x64 -o $INSTALL_PATH --self-contained

# Build HttPlaceholder GUI
GUI_PROJECT_PATH=$NEW_SOURCE_PATH/gui
cd "$GUI_PROJECT_PATH"
if [ -d "dist" ]; then
  echo "Deleting folder dist"
  sudo rm -r "dist"
fi

npm install
npm run build

GUI_PATH=$INSTALL_PATH/gui
if [ -d "$GUI_PATH" ]; then
  echo "Re-creating folder $GUI_PATH"
  sudo rm -r "$GUI_PATH"  
fi

sudo mkdir "$GUI_PATH"
sudo cp -r dist/. "$GUI_PATH"

# Build docs for use in UI
DOCS_SOURCE_DIR="$NEW_SOURCE_PATH/docs/httpl-docs"
GUI_DOCS_DIR="$GUI_PATH/docs"
if [ -d "$GUI_DOCS_DIR" ]; then
  echo "Deleting folder $GUI_DOCS_DIR"
  sudo rm -r "$GUI_DOCS_DIR"
fi

sudo mkdir "$GUI_DOCS_DIR"

echo "Building documentation"
cd "$DOCS_SOURCE_DIR"
pip install mkdocs
python3 sync.py
/home/vagrant/.local/bin/mkdocs build
sudo cp -r site/. "$GUI_DOCS_DIR"

# Move config to correct location
export DATA_PATH=/etc/httplaceholder
if [ ! -d "$DATA_PATH" ]; then
    echo "Creating path $DATA_PATH"
    sudo mkdir $DATA_PATH
fi

sudo cp $NEW_SOURCE_PATH/vagrant/linux/config.json $DATA_PATH

# Install HttPlaceholder systemd service
envsubst < $NEW_SOURCE_PATH/vagrant/linux/httplaceholder.service > /home/vagrant/httplaceholder.service
sudo cp /home/vagrant/httplaceholder.service /etc/systemd/system/httplaceholder.service 
sudo systemctl enable httplaceholder
sudo systemctl stop httplaceholder
sudo systemctl daemon-reload
sudo systemctl start httplaceholder

# Remove default site from Nginx
sudo rm /etc/nginx/sites-available/default
sudo rm /etc/nginx/sites-enabled/default

# Create self signed certificate for website
if [ ! -f "$DATA_PATH/nginx.key" ]; then
    sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -subj "/C=NL/ST=Drenthe/L=Eelde/O=IT/CN=localhost" -keyout $DATA_PATH/nginx.key -out $DATA_PATH/nginx.crt
fi

# Install HttPlaceholder reverse proxy site in Nginx
sudo cp $NEW_SOURCE_PATH/vagrant/linux/httplaceholder.conf /etc/nginx/sites-available
sudo ln -sf /etc/nginx/sites-available/httplaceholder.conf /etc/nginx/sites-enabled/httplaceholder.conf
sudo systemctl restart nginx