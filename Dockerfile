# Build API
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . ./
RUN VERSION=$(cat version.txt) && cd src/HttPlaceholder && dotnet publish -c Release /p:Version=$VERSION /p:AssemblyVersion=$VERSION /p:FileVersion=$VERSION  -o /app/out

# Build UI
FROM node:16 AS gui-build-env
WORKDIR /app

COPY . ./
RUN cd gui && npm install && npm run build

# Build docs
FROM python:slim-bullseye AS doc-build-env
WORKDIR /app
COPY . ./
RUN cd docs/httpl-docs && pip install mkdocs && python sync.py && mkdocs build && cp -r site /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=gui-build-env /app/gui/dist ./gui
COPY --from=doc-build-env /app/site/. ./gui/docs
ENV inputFile=/var/httplaceholder
RUN mkdir /var/httplaceholder
ENTRYPOINT ["dotnet", "HttPlaceholder.dll"]