# \*Unofficial\* Temporal 102 dotnet code solutions

This repo contains my unofficial .NET code solutions for the temporal 102 golang course.

At the time of writing no .NET course exists (as it's still in beta), but I've ported the golang course as a learning exercise. To the best of my knowledge the solutions are correct. Some of the exercises contain some minor modifications but I've included links to the original README's in each of the exercises.

Golang course link - https://github.com/temporalio/edu-102-go-code

## Prerequisites

- Temporal CLI - https://docs.temporal.io/cli
- .NET 7 - https://dotnet.microsoft.com/en-us/download/dotnet/7.0
- Your favourite IDE

> Note - I used VSCode to complete the course, but I've added all of the projects under exercises to the solution in the root of the repo if you'd prefer to open it in Rider or Visual Studio. Projects for each of the exercises are split into solution folders.

## Exercises

1 - [Using Structs](./exercises/usingstructs/README.md)

2 - [Durable Execution](./exercises/durableexecution/README.md)

3 - [Testing Code](./exercises/testingcode/README.md)

4 - [Debug Activity](./exercises//debugactivity/README.md)

5 - [Versioning Workflows](./exercises/versionworkflow/README.md)

## Repo structure

- `./exercises` contains the exercises from the course.
- Some of the exercises are split into `/practice` and `/solution` folders.
- The `/practice` folders contains a partially complete solution which you can complete yourself.
- The `/solution` folders contains a complete reference solution.

## Running the temporal-server

You'll need a temporal server implementation running for each of the exercises. The easiest way to do this is to use the temporal CLI. Run the below to start a dev server:

```sh
temporal server start-dev
```

> Note - when using the temporal CLI the server will be available at `localhost:7233` and the webUI at `http://localhost:8233/`
