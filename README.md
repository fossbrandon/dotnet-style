# Style

`dotnet-style` is a .NET tool that enables users to maintain a consistent and configurable C# code
style throughout projects.

It wraps existing and reputable tooling with the goal of providing a single interface for managing
C# code style functionality that would otherwise require running these other tools separately. As a
developer, this is less ideal because it requires me to remember the unique syntax and overall
knowledge for each of the separate tools rather than just learning and using this tool instead to
perform the same actions.

## Install

The .NET tool is available via [NuGet](https://www.nuget.org/packages/Style).

### Global Tool

If you want to run the .NET tool from any directory on your machine without specifying the tool's
location, install it as a global tool so that it is added to a directory visible to your PATH
environment variable.

Install `dotnet-style` as a global .NET tool using the command `dotnet tool install style --global`

### Local Tool

If you prefer to run the .NET tool from select directories, install it as a local tool. This can be
beneficial in instances such as working on a shared code repository where you want any developer who
clones the repo to have easy access to installing the same tools and versions as everyone else
working on the same code.

Install `dotnet-style` as a local .NET tool using the following steps:

1. Add a `dotnet-tools.json` manifest file in a `.config` directory under the current directory
   using the command `dotnet new tool-manifest` if the files does not currently exist.
2. Install the .NET tool using the command `dotnet tool install style` without any additional
   options.
   - This updates the `dotnet-tools.json` manifest file with information such as the tool's name and
     version so that all developers use the same tool.
3. Anyone who wants to use the local tool can run the command `dotnet tool restore` from within the
   repo to restore their local .NET tool(s) to match the manifest file.

   - 💡 `Quick Tip:` You can add this as an `MSBuild` `PreBuildEvent` so that devs don't have to
     worry about manually checking for changes and restoring tools when applicable.

     - Ex:

       ```xml
         <!-- Commands to run before builds -->
         <Target Name="Prebuild Actions" BeforeTargets="PreBuildEvent">
           <Exec Command="dotnet tool restore"/>
         </Target>
       ```

## Setup

`dotnet-style` wraps existing tools so you will need to setup the tools you plan to use along with
the code style configuration you want to enforce. More information regarding each configurable tool
is listed below:

- [dotnet-format](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format):
  - This tool is bundled with the `.NET 6 SDK` and later.
    - If you are using an older version of .NET you'll have to install an older version of the tool
      as a .NET tool.
  - This tool uses an [EditorConfig](https://editorconfig.org/) as the configurable source of truth
    for C# code style.
    - Create your own `.editorconfig` file with customized settings and severities if you want to
      manage code style using this tool.
      - Examples:
        - [Roslyn](https://github.com/dotnet/roslyn/blob/main/.editorconfig) has a pretty
          comprehensive and highly used EditorConfig.
        - Reference the `.editorconfig` files throughout
          [dotnet-style repository](https://github.com/fossbrandon/dotnet-style) for extremely
          customized (probably overkill 😁) examples where there is a default root configuration
          that is overridden by more specific configurations when needed.
- [CSharpier](https://csharpier.com/):
  - Install the tool following tool-specific documentation at the resource listed above.
  - Because this is an opinionated code formatter, it only allows for a few configuration options to
    be specified in a `.csharpierrc` file as explained in the tool-specific documentation.
    - Example: Reference the `.csharpierrc.yaml` file located in the the
      [dotnet-style repository](https://github.com/fossbrandon/dotnet-style) for an example of how
      that repository configures and uses the tool.

## Usage

After the `dotnet-style` is installed you can run the tool by typing `dotnet style` in a terminal to
run the tool as a CLI application.

### General CLI Usage

```text
Style v1.0.0
  A customizable dotnet tool that helps maintain a consistent C# coding style.

USAGE
  dotnet style [options]
  dotnet style [command] [...]

OPTIONS
  -h|--help         Shows help text.
  --version         Shows version information.

COMMANDS
  format            Formats C# files according to a defined coding style.
  verify            Verifies that C# files comply with the defined coding style.

You can run `dotnet style [command] --help` to show help on a specific command.
```

### Format Command Usage

```text
Style v1.0.0
  A customizable dotnet tool that helps maintain a consistent C# coding style.

USAGE
  dotnet style format [options]

DESCRIPTION
  Formats C# files according to a defined coding style.

OPTIONS
  -p|--path         The directory containing files to recursively format. Default: "C:\Users\branfoss\source\repos\Style\src".
  -s|--style        Whether to format code using the 'dotnet format' formatter to run code style analyzers and apply fixes. Default: "True".
  -a|--analyzers    Whether to format code using the 'dotnet format' formatter to run third party code style analyzers and apply fixes. Default: "True".
  -w|--whitespace   Whether to format code using the 'dotnet format' formatter to run whitespace formatting. Default: "False".
  -c|--csharpier    Whether to format code using the 'CSharpier' opinionated formatter. Note: If formatting with --whitespace, this option must be disabled to avoid conflicts as they both handle whitespace formatting. Default: "True".
  -v|--verbosity    The output verbosity level. Choices: "Quiet", "Normal", "Verbose". Default: "Normal".
  -h|--help         Shows help text.
```

### Verify Command Usage

```text
Style v1.0.0
  A customizable dotnet tool that helps maintain a consistent C# coding style.

USAGE
  dotnet style verify [options]

DESCRIPTION
  Verifies that C# files comply with the defined coding style.

OPTIONS
  -p|--path         The directory containing files to recursively verify the coding style compliance of. Default: "C:\Users\branfoss\source\repos\Style\src".
  -s|--style        Whether to verify code complies with the coding style used by the 'dotnet format' formatter for code style analyzer settings. Default: "True".
  -a|--analyzers    Whether to verify code complies with the coding style used by the 'dotnet format' formatter for third party code style analyzer settings. Default: "True".
  -w|--whitespace   Whether to verify code complies with the coding style used by the 'dotnet format' formatter for whitespace settings. Default: "False".
  -c|--csharpier    Whether to verify code complies with the coding style used by the 'CSharpier' opinionated formatter. Note: If verifying with --whitespace, this option must be disabled to avoid conflicts as they both handle whitespace formatting. Default: "True".
  -v|--verbosity    The output verbosity level. Choices: "Quiet", "Normal", "Verbose". Default: "Normal".
  -h|--help         Shows help text.
```

### Common Usage Examples

- Simple format usage: `dotnet style format`
  - This will recursively format C# files starting in your current directory using the C# formatters
    that are enabled by default.
- Customized format usage:
  `dotnet style format -p ./src -s true -a true -w true -c false -v verbose`
  - This will only format code in the .NET project defined within the relative `./src` directory
    using the `dotnet-format` formatter instead of the `CSharpier` formatter to avoid opinionated
    formatting. It also outputs additional information for potential debugging in the event there
    are errors or you want more details throughout the process.

**Note:** You can replace `format` with `verify` in any of the above examples to verify whether the
current C# files comply with coding standards rather than actually modifying any code.
