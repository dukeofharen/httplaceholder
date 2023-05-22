# Build docs
FROM python:slim-bullseye AS doc-build-env
WORKDIR /app
COPY . ./
RUN cd docs/httpl-docs && pip install mkdocs && python sync.py && mkdocs build && cp -r site /app

# Build UI
FROM node:18 AS gui-build-env
WORKDIR /app

COPY . ./
RUN cd gui && npm install && npm run build

# Build for Linux
FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY . ./
COPY --from=gui-build-env /app/gui/dist ./gui
COPY --from=doc-build-env /app/site/. ./gui/docs
RUN VERSION=$(cat version.txt) && \
    DIST_DIR=../../dist && \
    mkdir $DIST_DIR && \
    cd src/HttPlaceholder && \
    dotnet publish --configuration=release \
        --self-contained \
        --runtime=linux-x64 \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION \
        -o $DIST_DIR && \
    cp -r gui $DIST_DIR/gui && \
    cp -r ../../scripts/buildscript/installscripts/linux/. $DIST_DIR && \
    cp -r ../../docs $DIST_DIR && \
    cd $DIST_DIR && \
    tar -czvf ../httplaceholder_linux-x64.tar.gz .