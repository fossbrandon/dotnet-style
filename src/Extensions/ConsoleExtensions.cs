using CliFx.Infrastructure;
using CliWrap.Buffered;

namespace Style.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IConsole"/> interface.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Asynchronously writes the CLI command to run to the console stream.
    /// </summary>
    /// <param name="console">The <see cref="IConsole"/> to write to standard output to.</param>
    /// <param name="command">The CLI command to run.</param>
    /// <param name="arguments">The CLI command arguments to attach to the CLI command.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write the message.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">An empty parameter value was provided.</exception>
    public static async Task WriteCliCommandToRunAsync(
        this IConsole console,
        string command,
        string? arguments,
        Verbosity currentVerbosity
    )
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentNullException(
                nameof(command),
                "The parameter must be a non-empty value"
            );
        }

        await console.Output.WriteNormalLineAsync(
            $"Running the command '{command.Trim()}"
                + $"{(string.IsNullOrEmpty(arguments?.Trim()) ? "'" : $" {arguments.Trim()}'")}",
            currentVerbosity
        );
    }

    /// <summary>
    /// Asynchronously writes the standard output from a completed CLI command to the console stream.
    /// </summary>
    /// <param name="console">The <see cref="IConsole"/> to write to standard output to.</param>
    /// <param name="result">The CLI command result with available output and statistics.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write the message.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operations.</returns>
    public static async Task WriteCliCommandStandardOutputAsync(
        this IConsole console,
        BufferedCommandResult result,
        Verbosity currentVerbosity
    )
    {
        // Write standard output if it is available.
        if (!string.IsNullOrWhiteSpace(result.StandardOutput))
        {
            await console.Output.WriteVerboseLineAsync("Command Output:", currentVerbosity);
            await console.Output.WriteVerboseLineAsync("", currentVerbosity);

            // Switch the color of the output to help show it is not written by us.
            console.ForegroundColor = ConsoleColor.Blue;

            await console.Output.WriteVerboseLineAsync(
                result.StandardOutput.Trim(),
                currentVerbosity
            );
            await console.Output.WriteVerboseLineAsync("", currentVerbosity);

            // Reset the console colors now that we are writing our own output.
            console.ResetColor();
        }
    }

    /// <summary>
    /// Asynchronously writes a quiet string to the console stream, followed by a line terminator
    /// if the given maximum verbosity is greater than or equal to <see cref="Verbosity.Quiet"/>.
    /// </summary>
    /// <param name="console">The <see cref="ConsoleWriter"/> to write characters to the stream with.</param>
    /// <param name="message">The message to write to the stream.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write the message.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    public static Task WriteQuietLineAsync(
        this ConsoleWriter console,
        string? message,
        Verbosity currentVerbosity
    ) => WriteLineAsync(console, message, Verbosity.Quiet, currentVerbosity);

    /// <summary>
    /// Asynchronously writes a normal string to the console stream, followed by a line terminator
    /// if the given maximum verbosity is greater than or equal to <see cref="Verbosity.Normal"/>.
    /// </summary>
    /// <param name="console">The <see cref="ConsoleWriter"/> to write characters to the stream with.</param>
    /// <param name="message">The message to write to the stream.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write the message.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    public static Task WriteNormalLineAsync(
        this ConsoleWriter console,
        string? message,
        Verbosity currentVerbosity
    ) => WriteLineAsync(console, message, Verbosity.Normal, currentVerbosity);

    /// <summary>
    /// Asynchronously writes a verbose string to the console stream, followed by a line terminator if the
    /// given maximum verbosity is greater than or equal to <see cref="Verbosity.Verbose"/>.
    /// </summary>
    /// <param name="console">The <see cref="ConsoleWriter"/> to write characters to the stream with.</param>
    /// <param name="message">The message to write to the stream.</param>
    /// <param name="currentVerbosity">
    /// The output <see cref="Verbosity"/> level which determines whether to write the message.
    /// </param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    public static Task WriteVerboseLineAsync(
        this ConsoleWriter console,
        string? message,
        Verbosity currentVerbosity
    ) => WriteLineAsync(console, message, Verbosity.Verbose, currentVerbosity);

    private static async Task WriteLineAsync(
        ConsoleWriter console,
        string? message,
        Verbosity messageVerbosity,
        Verbosity currentVerbosity
    )
    {
        if (currentVerbosity >= messageVerbosity)
        {
            await console.WriteLineAsync(message);
        }
    }
}
