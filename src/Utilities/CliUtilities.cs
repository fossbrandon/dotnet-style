using CliFx.Exceptions;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;
using Style.Extensions;

namespace Style.Utilities;

/// <summary>
/// Provides helpful methods to assist with CLI operations.
/// </summary>
public class CliUtilities
{
    /// <summary>
    /// Provides a wrapper method for running CLI commands in a consistent way.
    /// </summary>
    /// <param name="console">The <see cref="IConsole"/> to write to standard output to.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write a message.
    /// </param>
    /// <param name="cliExecutablePath">The path to the CLI command to run.</param>
    /// <param name="arguments">The arguments to run the CLI command with.</param>
    /// <param name="workingDirectory">The directory path from which to run the CLI command.</param>
    /// <param name="ct"></param>
    /// <returns>A <see cref="BufferedCommandResult"/> for the executed command.</returns>
    public static async ValueTask<BufferedCommandResult> RunCliCommandAsync(
        IConsole console,
        Verbosity currentVerbosity,
        string cliExecutablePath,
        string arguments,
        string workingDirectory,
        CancellationToken ct = default
    )
    {
        await console.WriteCliCommandToRunAsync(cliExecutablePath, arguments, currentVerbosity);

        return await Cli.Wrap(cliExecutablePath)
            .WithArguments(arguments)
            .WithWorkingDirectory(workingDirectory)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(ct);
    }

    /// <summary>
    /// Evaluates whether the CLI command was successful.
    /// </summary>
    /// <param name="commandResult">The result of the CLI command.</param>
    /// <returns>True if the CLI command has a 0 exit code, otherwise false.</returns>
    public static bool IsCliCommandSuccessful(BufferedCommandResult commandResult) =>
        commandResult.ExitCode is 0;

    /// <summary>
    /// Provides consistent handling of a CLI command result.
    /// </summary>
    /// <param name="console">The <see cref="IConsole"/> to write to standard output to.</param>
    /// <param name = "currentVerbosity" >
    /// The output <see cref="Verbosity"/> level which determines whether to write a message.
    /// </param>
    /// <param name="commandResult">The result of the CLI command.</param>
    /// <param name="buildExceptionMessage">The message to display in the event of an exception.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    /// <exception cref="CommandException">The command result shows a non-zero exit code.</exception>
    public static async ValueTask HandleCliResultAsync(
        IConsole console,
        Verbosity currentVerbosity,
        BufferedCommandResult commandResult,
        Func<string, string> buildExceptionMessage
    )
    {
        await console.WriteCliCommandStandardOutputAsync(commandResult, currentVerbosity);

        if (!IsCliCommandSuccessful(commandResult))
        {
            throw new CommandException(buildExceptionMessage(commandResult.StandardError));
        }

        await console.Output.WriteNormalLineAsync("Success", currentVerbosity);
    }

    /// <summary>
    /// Gets a message to provide users who are not specifying verbose verbosity if they could
    /// benefit from doing so.
    /// </summary>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether this message should be given.
    /// </param>
    /// <returns>A helpful message if using a lower verbosity, otherwise nothing.</returns>
    public static string GetUseHigherVerbosityMessage(Verbosity currentVerbosity) =>
        currentVerbosity < Verbosity.Verbose ? Constants.UseHigherVerbosityMessage : "";
}
