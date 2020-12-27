# Docker instructions

HttPlaceholder has a Docker image; it can be found [here](https://hub.docker.com/r/dukeofharen/httplaceholder). This page explains some basics about the Docker image and some examples you can use.

## Basic example

This is a very basic example for running HttPlaceholder locally from the command line.

`docker run -p 5000:5000 dukeofharen/httplaceholder:latest`

HttPlaceholder can now be reached on `http://localhost:5000` (or `http://localhost:5000/ph-ui` to get to the management interface).

## Configuration

The Docker container uses the configuration values as specified [here](CONFIG.md). Here is an example of starting the HttPlaceholder container with different ports for HTTP and HTTPS:

`docker run -p 8080:8080 -p 4430:4430 --env port=8080 --env httpsPort=4430 dukeofharen/httplaceholder:latest`

## Docker Compose examples

In the links below, you'll find several hosting scenario's written for Docker Compose. If you want to run any of these examples on your PC, open your terminal, go to one of the folders that contain a `docker-compose.yml` example and run `docker-compose up`.

- [Hosting with file storage](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-file-storage)
- [Hosting with SQL Server](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-mssql)
- [Hosting with MySQL](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-mysql)
- [Hosting with SQLite](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-sqlite)
- [Hosting with .yml stub files](https://github.com/dukeofharen/httplaceholder/tree/master/docker/httplaceholder-with-stub-files)