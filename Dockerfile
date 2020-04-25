# Build API
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-buster AS build-env
WORKDIR /app

COPY . ./
RUN cd src/HttPlaceholder && dotnet publish -c Release -o /app/out

# Build UI
FROM node:14.0.0 AS gui-build-env
WORKDIR /app

COPY . ./
RUN cd gui && npm install && npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-buster
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=gui-build-env /app/gui/dist ./gui
ENTRYPOINT ["dotnet", "HttPlaceholder.dll", "-V"]