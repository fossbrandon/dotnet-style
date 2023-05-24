using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using CliWrap;
using Style.Attributes;
using Style.Extensions;
using Style.Utilities;

namespace Style.Verify;

/// <summary>
/// Models the verify command which verifies whether C# files currently comply with code style standards.
/// </summary>
[Command(
    Constants.VerifyCommand,
    Description = "Verifies that C# files comply with the defined coding style."
)]
public class VerifyCommand : ICommand
{
    /// <summary>
    /// Gets or initializes the path option which specifies the directory to recurisvely verify the
    /// coding style compliance of.
    /// </summary>
    [CommandOption(
        Constants.PathOption,
        'p',
        Description = "The directory containing files "
            + "to recursively verify the coding style compliance of.",
        IsRequired = false
    )]
    public DirectoryInfo RootFormatPath { get; init; } =
        new DirectoryInfo(Directory.GetCurrentDirectory());

    /// <summary>
    /// Gets or initializes the verify with 'dotnet format style --verify-no-changes' option.
    /// </summary>
    [CommandOption(
        Constants.StyleOption,
        's',
        Description = "Whether to verify code "
            + "complies with the coding style used by the 'dotnet format' formatter for code style "
            + "analyzer settings.",
        IsRequired = false
    )]
    [FormatterOption(Constants.StyleOption)]
    public bool FormatStyle { get; init; } = true;

    /// <summary>
    /// Gets or initializes the verify with 'dotnet format analyzers --verify-no-changes' option.
    /// </summary>
    [CommandOption(
        Constants.AnalyzersOption,
        'a',
        Description = "Whether to verify code "
            + "complies with the coding style used by the 'dotnet format' formatter for third party "
            + "code style analyzer settings.",
        IsRequired = false
    )]
    [FormatterOption(Constants.AnalyzersOption)]
    public bool FormatAnalyzers { get; init; } = true;

    /// <summary>
    /// Gets or initializes the verify with 'dotnet format whitespace --verify-no-changes' option.
    /// </summary>
    [CommandOption(
        Constants.WhitespaceOption,
        'w',
        Description = "Whether to verify code "
            + "complies with the coding style used by the 'dotnet format' formatter for whitespace settings.",
        IsRequired = false
    )]
    [FormatterOption(Constants.WhitespaceOption)]
    public bool FormatWhitespace { get; init; } = false;

    /// <summary>
    /// Gets or initializes the verify with 'dotnet csharpier --check' option.
    /// </summary>
    [CommandOption(
        Constants.CsharpierOption,
        'c',
        Description = "Whether to verify code "
            + "complies with the coding style used by the 'CSharpier' opinionated formatter. Note: If verifying with "
            + $"--{Constants.WhitespaceOption}, this option must be disabled to avoid conflicts as they "
            + "both handle whitespace formatting.",
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
                "Verifying that C# files currently comply "
                    + $"with defined style standards within '{RootFormatPath.FullName}'",
                OutputVerbosity
            );

            // Add cancellation token support.
            var ct = console.RegisterCancellationHandler();

            await VerifyAsync(console, ct);

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
        var formatterOptionProperties = typeof(VerifyCommand)
            .GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(FormatterOptionAttribute)));

        // Ensure that at least one formatter is enabled.
        if (!formatterOptionProperties.Any(p => p.GetValue(this) is true))
        {
            throw new CommandException(
                "You must enable at least one formatter to verify the code " + "style with.",
                showHelp: true
            );
        }

        // Ensure that CSharpier is not specified at the same time as 'dotnet format whitespace'.
        if (FormatCsharpier && FormatWhitespace)
        {
            throw new CommandException(
                "You may only enable one whitespace formatter to verify code "
                    + $"style compliance with by specifying either the '--{Constants.CsharpierOption}' "
                    + $"option or the '--{Constants.WhitespaceOption}' option to avoid potential conflicts.",
                showHelp: true
            );
        }
    }

    private async Task VerifyAsync(IConsole console, CancellationToken ct = default)
    {
        if (FormatStyle)
        {
            var arguments = $"format style . --verify-no-changes";
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
            var arguments = $"format analyzers . --verify-no-changes";
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
            var arguments = $"format whitespace . --verify-no-changes";
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
            var arguments = $"csharpier . --check";
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
            ? $"Code does not comply with the defined style standards.{CliUtilities.GetUseHigherVerbosityMessage(OutputVerbosity)}"
            : $"Code does not comply with the defined style standards or an error occurred.{Environment.NewLine}{Environment.NewLine}"
                + $"Standard Error:{Environment.NewLine}{Environment.NewLine}"
                + $" {stdError}{Environment.NewLine}{Environment.NewLine}"
                + CliUtilities.GetUseHigherVerbosityMessage(OutputVerbosity);
}
