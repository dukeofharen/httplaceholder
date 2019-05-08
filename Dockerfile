FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./
RUN cd src/HttPlaceholder && dotnet publish -c Release -o ../../out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "HttPlaceholder.dll", "--fileStorageLocation", "/app/stubs"]