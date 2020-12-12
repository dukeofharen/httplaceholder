# Build API
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN VERSION=$(cat version.txt) && cd src/HttPlaceholder && dotnet publish -c Release /p:Version=$VERSION /p:AssemblyVersion=$VERSION /p:FileVersion=$VERSION  -o /app/out

# Build UI
FROM node:14.0.0 AS gui-build-env
WORKDIR /app

COPY . ./
RUN cd gui && npm install && npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=gui-build-env /app/gui/dist ./gui
ENTRYPOINT ["dotnet", "HttPlaceholder.dll", "-V"]