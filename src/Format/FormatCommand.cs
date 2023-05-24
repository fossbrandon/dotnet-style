using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using CliWrap;
using Style.Attributes;
using Style.Extensions;
using Style.Utilities;

namespace Style.Format;

/// <summary>
/// Models the format command which formats C# files to comply with code style standards.
/// </summary>
[Command(
    Constants.FormatCommand,
    Description = "Formats C# files according to a defined coding style."
)]
public class FormatCommand : ICommand
{
    /// <summary>
    /// Gets or initializes the path option which specifies the directory to recurisvely format.
    /// </summary>
    [CommandOption(
        Constants.PathOption,
        'p',
        Description = "The directory containing files " + "to recursively format.",
        IsRequired = false
    )]
    public DirectoryInfo RootFormatPath { get; init; } =
        new DirectoryInfo(Directory.GetCurrentDirectory());

    /// <summary>
    /// Gets or initializes the format with 'dotnet format style' option.
    /// </summary>
    [CommandOption(
        Constants.StyleOption,
        's',
        Description = "Whether to format code "
            + "using the 'dotnet format' formatter to run code style analyzers and apply fixes.",
        IsRequired = false
    )]
    [FormatterOption(Constants.StyleOption)]
    public bool FormatStyle { get; init; } = true;

    /// <summary>
    /// Gets or initializes the format with 'dotnet format analyzers' option.
    /// </summary>
    [CommandOption(
        Constants.AnalyzersOption,
        'a',
        Description = "Whether to format code "
            + "using the 'dotnet format' formatter to run third party code style analyzers and apply fixes.",
        IsRequired = false
    )]
    [FormatterOption(Constants.AnalyzersOption)]
    public bool FormatAnalyzers { get; init; } = true;

    /// <summary>
    /// Gets or initializes the format with 'dotnet format whitespace' option.
    /// </summary>
    [CommandOption(
        Constants.WhitespaceOption,
        'w',
        Description = "Whether to format code "
            + "using the 'dotnet format' formatter to run whitespace formatting.",
        IsRequired = false
    )]
    [FormatterOption(Constants.WhitespaceOption)]
    public bool FormatWhitespace { get; init; } = false;

    /// <summary>
    /// Gets or initializes the format with 'dotnet csharpier' option.
    /// </summary>
    [CommandOption(
        Constants.CsharpierOption,
        'c',
        Description = "Whether to format code "
            + $"using the 'CSharpier' opinionated formatter. Note: If formatting with --{Constants.WhitespaceOption}, "
            + "this option must be disabled to avoid conflicts as they both handle whitespace formatting.",
        IsRequired = false
    )]
    [FormatterOption(Constants.CsharpierOption)]
    public bool FormatCsharpier { get; init; } = true;

    /// <summary>
    /// Gets or initializes the output verbosity option.
    /// </summary>
    [CommandOption(
        Constants.VerbosityOption,
        'v',
        Description = "The output verbosity level.",
        IsRequired = false
    )]
    public Verbosity OutputVerbosity { get; init; } = Verbosity.Normal;

    /// <inheritdoc/>
    public async ValueTask ExecuteAsync(IConsole console)
    {
        try
        {
            ValidateCommandOptions();

            await console.Output.WriteNormalLineAsync(
                "Formatting C# files within " + $"'{RootFormatPath.FullName}'",
                OutputVerbosity
            );

            // Add cancellation token support.
            var ct = console.RegisterCancellationHandler();

            await FormatAsync(console, ct);

            await console.Output.WriteNormalLineAsync("Done", OutputVerbosity);
        }
        // Rethrow a command exception as is.
        catch (CommandException)
        {
            throw;
        }
        // Wrap an unexpected exception with helpful text.
        catch (Exception ex)
        {
            throw new CommandException(
                $"The following error has occurred:{Environment.NewLine}"
                    + $"  {ex.Message}{Environment.NewLine}"
                    + "Double-check the command options and try again.",
                exitCode: 1,
                showHelp: true,
                innerException: ex
            );
        }
    }

    private void ValidateCommandOptions()
    {
        var formatterOptionProperties = typeof(FormatCommand)
            .GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(FormatterOptionAttribute)));

        // Ensure that at least one formatter is enabled.
        if (!formatterOptionProperties.Any(p => p.GetValue(this) is true))
        {
            throw new CommandException(
                "You must enable at least one formatter to format code with.",
                showHelp: true
            );
        }

        // Ensure that CSharpier is not specified at the same time as 'dotnet format whitespace'.
        if (FormatCsharpier && FormatWhitespace)
        {
            throw new CommandException(
                "You may only enable one whitespace formatter to format code with "
                    + $"by specifying either the '--{Constants.CsharpierOption}' option or the "
                    + $"'--{Constants.WhitespaceOption}' option to avoid potential conflicts.",
                showHelp: true
            );
        }
    }

    private async Task FormatAsync(IConsole console, CancellationToken ct = default)
    {
        if (FormatStyle)
        {
            var arguments = $"format style .";
            var result = await CliUtilities.RunCliCommandAsync(
                console,
                OutputVerbosity,
                Constants.DotnetCli,
                arguments,
                RootFormatPath.FullName,
                ct
            );
            await CliUtilities.HandleCliResultAsync(
                console,
                OutputVerbosity,
                result,
                BuildVerificationFailureExceptionMessage
            );
        }

        if (FormatAnalyzers)
        {
            var arguments = $"format analyzers .";
            var result = await CliUtilities.RunCliCommandAsync(
                console,
                OutputVerbosity,
                Constants.DotnetCli,
                arguments,
                RootFormatPath.FullName,
                ct
            );
            await CliUtilities.HandleCliResultAsync(
                console,
                OutputVerbosity,
                result,
                BuildVerificationFailureExceptionMessage
            );
        }

        if (FormatWhitespace)
        {
            var arguments = $"format whitespace .";
            var result = await CliUtilities.RunCliCommandAsync(
                console,
                OutputVerbosity,
                Constants.DotnetCli,
                arguments,
                RootFormatPath.FullName,
                ct
            );
            await CliUtilities.HandleCliResultAsync(
                console,
                OutputVerbosity,
                result,
                BuildVerificationFailureExceptionMessage
            );
        }

        if (FormatCsharpier)
        {
            var arguments = $"csharpier .";
            var result = await CliUtilities.RunCliCommandAsync(
                console,
                OutputVerbosity,
                Constants.DotnetCli,
                arguments,
                RootFormatPath.FullName,
                ct
            );
            await CliUtilities.HandleCliResultAsync(
                console,
                OutputVerbosity,
                result,
                BuildVerificationFailureExceptionMessage
            );
        }
    }

    private string BuildVerificationFailureExceptionMessage(string stdError) =>
        string.IsNullOrWhiteSpace(stdError)
            ? $"The command returned a non-zero exit code.{CliUtilities.GetUseHigherVerbosityMessage(OutputVerbosity)}"
            : $"The command returned a non-zero exit code.{Environment.NewLine}{Environment.NewLine}"
                + $"Standard Error:{Environment.NewLine}{Environment.NewLine}"
                + $" {stdError}{Environment.NewLine}{Environment.NewLine}"
                + CliUtilities.GetUseHigherVerbosityMessage(OutputVerbosity);
}
