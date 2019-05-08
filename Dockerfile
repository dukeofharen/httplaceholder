# Build API
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./
RUN cd src/HttPlaceholder && dotnet publish -c Release -o ../../out

# Build UI
FROM node AS gui-build-env
WORKDIR /app

COPY . ./
RUN cd gui && npm install && npm run build

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=gui-build-env /app/gui/dist ./gui
ENTRYPOINT ["dotnet", "HttPlaceholder.dll", "--fileStorageLocation", "/app/stubs"]