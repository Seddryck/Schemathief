# Schemathief

![Logo](https://raw.githubusercontent.com/Seddryck/Schemathief/main/assets/schemathief-logo-256.png)

Schemathief is a lightweight .NET library and CLI tool for generating JSON-Schema delta documents from your CLR types. Whether you need to keep your API schema up-to-date, drive automated data-model documentation, or generate client stubs only for the new fields you’ve added, Schemathief does the heavy lifting:

1. Load & compare

Fetches a base schema from a URL or file via a pluggable IBaseSchemaLoader

Reflects over your target assembly to discover all public, read/write properties

Allows you to exclude known fields (e.g. legacy or deprecated properties)

1. Generate delta

Maps .NET types to JSON-Schema types (string, integer, array, object) with full support for nullable, primitive, and complex types

Recursively builds nested definitions for child objects and collections

Produces a minimal “delta schema” that you can combine with your base via an allOf merge

1. CLI & API

A dotnet tool–friendly System.CommandLine implementation (delta verb, intuitive options) ready for CI/CD

Dependency-injection–friendly services (IDeltaService, IBaseSchemaLoader) so you can stub or replace behavior in your own apps

Fully unit‐tested core (SchemaBuilder, TypeInspector, ClrToJsonTypeMapper, TypeInspectorHelper) and integration‐tested end-to-end

1. Pack & ship

Ship your own JSON-Schema artifacts right inside your NuGet package via contentFiles/any/any/schemas so consumers get editor IntelliSense out of the box

Integrates smoothly into existing build pipelines with dotnet pack, GitVersion, or any other automation

# Why Schemathief?

Zero boilerplate: No more hand-crafting schema diffs by hand or writing reflection glue in every project.

Future-proof: Designed around the JSON-Schema draft-07 spec today, easy to extend for draft-next tomorrow.

Give it a try in your next microservice or data-model repo—keep your schemas in lock-step with your code

[About][] | [Installing][] | [Quickstart][]

[About]: #about (About)
[Installing]: #installing (Installing)
[Quickstart]: #quickstart (Quickstart)

## About

**Social media:** [![website](https://img.shields.io/badge/website-seddryck.github.io/Schemathief-fe762d.svg)](https://seddryck.github.io/Schemathief)
[![twitter badge](https://img.shields.io/badge/twitter%20Schemathief-@Seddryck-blue.svg?style=flat&logo=twitter)](https://twitter.com/Seddryck)

**Releases:** [![GitHub releases](https://img.shields.io/github/v/release/seddryck/schemathief?label=GitHub%20releases)](https://github.com/seddryck/schemathief/releases/latest) 
[![nuget](https://img.shields.io/nuget/v/Schemathief-cli.svg)](https://www.nuget.org/packages/Schemathief-cli/) [![Docker Image Version](https://img.shields.io/docker/v/seddryck/schemathief?label=docker%20hub&color=0db7ed)](https://hub.docker.com/repository/docker/seddryck/schemathief/) [![GitHub Release Date](https://img.shields.io/github/release-date/seddryck/Schemathief.svg)](https://github.com/Seddryck/Schemathief/releases/latest) [![licence badge](https://img.shields.io/badge/License-Apache%202.0-yellow.svg)](https://github.com/Seddryck/Schemathief/blob/master/LICENSE) 

**Dev. activity:** [![GitHub last commit](https://img.shields.io/github/last-commit/Seddryck/Schemathief.svg)](https://github.com/Seddryck/Schemathief/commits)
![Still maintained](https://img.shields.io/maintenance/yes/2025.svg)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Seddryck/Schemathief)

**Continuous integration builds:** [![Build status](https://ci.appveyor.com/api/projects/status/srnux32j07ysvsp1?svg=true)](https://ci.appveyor.com/project/Seddryck/Schemathief/)
[![Tests](https://img.shields.io/appveyor/tests/seddryck/Schemathief.svg)](https://ci.appveyor.com/project/Seddryck/Schemathief/build/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/seddryck/Schemathief/badge)](https://www.codefactor.io/repository/github/seddryck/Schemathief)
[![codecov](https://codecov.io/github/Seddryck/Schemathief/branch/main/graph/badge.svg?token=76Q2XAL4J7)](https://codecov.io/github/Seddryck/Schemathief)
<!-- [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSeddryck%2FSchemathief.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSeddryck%2FSchemathief?ref=badge_shield) -->

**Status:** [![stars badge](https://img.shields.io/github/stars/Seddryck/Schemathief.svg)](https://github.com/Seddryck/Schemathief/stargazers)
[![Bugs badge](https://img.shields.io/github/issues/Seddryck/Schemathief/bug.svg?color=red&label=Bugs)](https://github.com/Seddryck/Schemathief/issues?utf8=%E2%9C%93&q=is:issue+is:open+label:bug+)
[![Top language](https://img.shields.io/github/languages/top/seddryck/Schemathief.svg)](https://github.com/Seddryck/Schemathief/search?l=C%23)

## Installing

### Install as a .NET global tool

A .NET global tool is a console application that you can install and run from any directory on your machine. Here’s a guide on how to perform a global installation of a .NET tool:

#### Prerequisites
Before installing a .NET global tool, you must have the .NET SDK installed on your machine. You can check if it's installed by running the following command in your terminal or Command Prompt:

```bash
dotnet --version
```
If .NET is not installed, download it from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet).

#### Install a .NET Global Tool
To install a .NET global tool, you use the dotnet tool install command. This command installs a tool for all users globally on your system.

```bash
dotnet tool install -g Schemathief-cli
```

`-g`: This flag tells the dotnet command to install the tool globally.

#### Verify Installation

After installing the tool, you can verify that it's available globally by running it from any directory.

```bash
schemathief --version
```

This command will display the installed tool’s version if the installation was successful.

#### Update a .NET Global Tool

To update a globally installed .NET tool, use the dotnet tool update command:

```bash
dotnet tool update -g Schemathief-cli
```

## Quickstart

### `delta` Command – Generate a JSON Schema Delta

Generates a **delta JSON Schema** from a .NET type by comparing it against a base schema. This is useful when evolving data models incrementally, and you want to isolate new or changed fields.

#### Usage

```bash
schemathief delta --assembly <path> --class <fqcn> --base <url> [--exclude <a|b>] [--output <file>]
```

You can also use short options:

```bash
schemathief delta -a MyLib.dll -c My.Namespace.Type -b https://schemas/base.json -x Id|Timestamp -o delta.json
```

#### Options

| Option            | Aliases         | Required | Description |
|-------------------|------------------|----------|-------------|
| `--assembly`      | `-a`             | ✅ Yes   | Path to the compiled `.dll` file containing the class. |
| `--class`         | `-c`             | ✅ Yes   | Fully qualified name of the target .NET class (e.g. `MyApp.Models.Customer`). |
| `--base`          | `-b`             | ✅ Yes   | URL or file path to the base JSON Schema you want to extend. |
| `--exclude`       | `-x`             | No       | Pipe-separated list of properties to ignore in the delta (e.g. `Id|Timestamp`). |
| `--output`        | `-o`             | No       | File path to write the generated delta schema. If omitted, output is written to the console. |

#### Examples

##### Output to console:

```bash
schemathief delta -a bin/Release/MyApi.dll -c MyApi.Models.Person -b https://schemas.example.com/person.base.json
```

##### Output to file with exclusions:

```bash
schemathief delta -a MyApi.dll -c MyApi.Models.Invoice -b base-schema.json -x "CreatedAt|Id" -o invoice.delta.schema.json
```

##### Notes

- If the generated delta schema is empty (i.e. all properties are already in the base schema or excluded), the CLI will emit:  

  ```
  No delta schema generated.
  ```

- The delta is combined with the base using an `allOf` JSON Schema construct.
