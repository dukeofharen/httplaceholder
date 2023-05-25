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

# Run .NET unit tests
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS test-env
WORKDIR /app
COPY . ./
RUN cd src && dotnet test

# Build .NET client
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dotnet-nuget-build-env
WORKDIR /app
COPY . ./
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    DIST_DIR=$ROOT_DIR/dist && \
    mkdir $DIST_DIR && \
    cd $ROOT_DIR/src/HttPlaceholder.Client && \
    dotnet pack -c Release \
        -o $DIST_DIR \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION

# Create OpenAPI file
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS swagger-build-env
WORKDIR /app
COPY . ./
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    DIST_DIR=$ROOT_DIR/dist && \
    mkdir $DIST_DIR && \
    cd $ROOT_DIR/src/HttPlaceholder.SwaggerGenerator && \
    dotnet run -c Release && \
    cp bin/Release/net7.0/swagger.json $DIST_DIR

# Build for Linux
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS linux-app-build-env
WORKDIR /app
COPY . ./
COPY --from=gui-build-env /app/gui/dist ./gui-dist
COPY --from=doc-build-env /app/site/. ./gui/docs
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    GUI_DIR=$ROOT_DIR/gui-dist && \
    DIST_DIR=$ROOT_DIR/dist && \
    DOCS_DIR=$ROOT_DIR/docs && \
    cd $ROOT_DIR/src/HttPlaceholder && \
    mkdir $DIST_DIR && \
    dotnet publish --configuration=release \
        --self-contained \
        --runtime=linux-x64 \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION \
        -o $DIST_DIR && \
    rm -rf $DIST_DIR/gui && \
    cp -r $GUI_DIR $DIST_DIR/gui && \
    cp -r $ROOT_DIR/scripts/buildscript/installscripts/linux/. $DIST_DIR && \
    cp -r $DOCS_DIR $DIST_DIR

# Build for Mac OSX
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS mac-app-build-env
WORKDIR /app
COPY . ./
COPY --from=gui-build-env /app/gui/dist ./gui-dist
COPY --from=doc-build-env /app/site/. ./gui/docs
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    GUI_DIR=$ROOT_DIR/gui-dist && \
    DIST_DIR=$ROOT_DIR/dist && \
    DOCS_DIR=$ROOT_DIR/docs && \
    cd $ROOT_DIR/src/HttPlaceholder && \
    mkdir $DIST_DIR && \
    dotnet publish --configuration=release \
        --self-contained \
        --runtime=osx-x64 \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION \
        -o $DIST_DIR && \
    rm -rf $DIST_DIR/gui && \
    cp -r $GUI_DIR $DIST_DIR/gui && \
    cp -r $ROOT_DIR/scripts/buildscript/installscripts/mac/. $DIST_DIR && \
    cp -r $DOCS_DIR $DIST_DIR

# Build for Windows
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS windows-app-build-env
WORKDIR /app
COPY . ./
COPY --from=gui-build-env /app/gui/dist ./gui-dist
COPY --from=doc-build-env /app/site/. ./gui/docs
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    GUI_DIR=$ROOT_DIR/gui-dist && \
    DIST_DIR=$ROOT_DIR/dist && \
    DOCS_DIR=$ROOT_DIR/docs && \
    cd $ROOT_DIR/src/HttPlaceholder && \
    mkdir $DIST_DIR && \
    dotnet publish --configuration=release \
        --self-contained \
        --runtime=win-x64 \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION \
        -o $DIST_DIR && \
    rm -rf $DIST_DIR/gui && \
    cp -r $GUI_DIR $DIST_DIR/gui && \
    cp -r $ROOT_DIR/scripts/buildscript/installscripts/windows/. $DIST_DIR && \
    cp -r $DOCS_DIR $DIST_DIR && \
    mv $DIST_DIR/web.config $DIST_DIR/_web.config

# Build tool
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS tool-build-env
WORKDIR /app
COPY . ./
COPY --from=gui-build-env /app/gui/dist ./gui-dist
COPY --from=doc-build-env /app/site/. ./gui/docs
RUN VERSION=$(cat version.txt) && \
    ROOT_DIR=/app && \
    GUI_DIR=$ROOT_DIR/gui-dist && \
    DIST_DIR=$ROOT_DIR/dist && \
    DOCS_DIR=$ROOT_DIR/docs && \
    cd $ROOT_DIR/src/HttPlaceholder && \
    mkdir $DIST_DIR && \
    rm -rf gui && \
    sed -i 's/<PackAsTool>false<\/PackAsTool>/<PackAsTool>true<\/PackAsTool>/' HttPlaceholder.csproj && \
    dotnet pack -c Tool \
        -o $DIST_DIR \
        /p:Version=$VERSION \
        /p:AssemblyVersion=$VERSION \
        /p:FileVersion=$VERSION

# Pack everything up
FROM ubuntu:22.04
WORKDIR /app
COPY . ./
COPY --from=linux-app-build-env /app/dist ./linux
COPY --from=mac-app-build-env /app/dist ./mac
COPY --from=windows-app-build-env /app/dist ./windows
COPY --from=tool-build-env /app/dist ./tool
COPY --from=dotnet-nuget-build-env /app/dist ./nuget
COPY --from=swagger-build-env /app/dist ./swagger
RUN mkdir dist && \
    apt update && \
    apt install zip -y && \
    ROOT_DIR=$(pwd) && \
    cd $ROOT_DIR/linux && \
    tar -cvzf ../dist/httplaceholder_linux-x64.tar.gz . && \
    cd $ROOT_DIR/mac && \
    tar -cvzf ../dist/httplaceholder_osx-x64.tar.gz . && \
    cd $ROOT_DIR/windows && \
    zip -r ../dist/httplaceholder_win-x64.zip . && \
    cd $ROOT_DIR/tool && \
    cp -r *.nupkg ../dist && \
    cd $ROOT_DIR/nuget && \
    cp -r *.nupkg ../dist && \
    cd $ROOT_DIR/swagger && \
    cp swagger.json ../dist && \
    cd $ROOT_DIR/docs && \
    tar -cvzf ../dist/docs.tar.gz .