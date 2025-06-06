# Contributing

When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other
method with the owners of this repository before making a change.
Please note we have a [code of conduct](CODE_OF_CONDUCT.md), please follow it in all your interactions with the project.

## Development environment setup

To set up a development environment, please follow these steps:

**Clone the repo**

   ```sh
   git clone https://github.com/dukeofharen/httplaceholder
   ```

### Setting up development environment for HttPlaceholder

1. Make sure [.NET 9](https://dot.net) is installed.
2. In your terminal, go to the folder `src`.
3. In the folder, executed the command `dotnet build`. This will download all NuGet packages and makes sure everything
   builds as intended.
4. Go to the folder `src/HttPlaceholder`.
5. Execute the command `dotnet run`. The HttPlaceholder application will now run and you can interact with it. To learn
   more about how to start the application with different configurations, go to the
   page [configuration](docs.md#configuration). The application runs, by default, on addresses <http://localhost:5000>
   and <https://localhost:5050>.
6. Ideally, you want to use an IDE or an advanced text editor to make changes to the source code. Any of the following (
   and probably more) IDEs / text editors are supported and provide full-fledged C#/.NET support:
    - [Visual Studio (Windows only)](https://visualstudio.microsoft.com/)
    - [Visual Studio Code (cross-platform)](https://code.visualstudio.com/)
    - [JetBrains Rider (cross-platform)](https://www.jetbrains.com/rider/)

### Setting up development environment for the user interface

1. Make sure [Node.js](https://nodejs.org/en/) is installed.
2. In your terminal, go to the folder `gui` folder.
3. Execute the command `npm install`. This will download all necessary packages for the frontend application.
4. When everything is downloaded, executed the command `npm run serve`. The application will be built and can be reached
   on <http://localhost:10001> (new UI).
5. When developing, make sure HttPlaceholder itself is also running (see the previous paragraph).
6. Ideally, you want to use an IDE or an advanced text editor to make changes to the source code. Any of the following (
   and probably more) IDEs / text editors are supported and provide full-fledged Vue / JavaScript support:
    - [Visual Studio (Windows only)](https://visualstudio.microsoft.com/)
    - [Visual Studio Code (cross-platform)](https://code.visualstudio.com/)
    - [JetBrains Webstorm (cross-platform)](https://www.jetbrains.com/webstorm/)

## Issues and feature requests

You've found a bug in the source code, a mistake in the documentation, or maybe you'd like a new feature? You can help
us by submitting an issue to our [GitHub Repository](https://github.com/dukeofharen/httplaceholder/issues). Before you
create an issue, make sure you search the archive, maybe your question was already answered.
Also, please check out [GitHub discussions](https://github.com/dukeofharen/httplaceholder/discussions) before submitting
an issue.

Please try to create bug reports that are:

- _Reproducible._ Include steps to reproduce the problem.
- _Specific._ Include as much detail as possible: which version, what environment, etc.
- _Unique._ Do not duplicate existing opened issues.
- _Scoped to a Single Bug._ One bug per report.

Even better: You could submit a pull request with a fix or new feature!

## Pull request process

1. Search our repository for open or closed
   [pull requests](https://github.com/dukeofharen/httplaceholder/pulls)
   that relates to your submission. You don't want to duplicate effort.
2. Fork the project
3. Create your feature branch (`git checkout -b feat/amazing_feature`)
4. Commit your changes (`git commit -m 'feat: add amazing_feature'`)
5. Push to the branch (`git push origin feat/amazing_feature`)
6. Open a pull request