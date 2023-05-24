using CliFx;
using CliFx.Infrastructure;

namespace Style.Tests.Utilities;

/// <summary>
/// Provides a collection of helper methods to assist tests.
/// </summary>
internal sealed class TestHelpers
{
    /// <summary>
    /// Builds a <see cref="CliApplication"/> suitable for running tests against.
    /// </summary>
    /// <param name="console"></param>
    /// <returns></returns>
    internal static CliApplication BuildStyleTestApp(IConsole console) =>
        new CliApplicationBuilder()
            .SetTitle(Constants.CliTitle)
            .SetExecutableName(Constants.CliExecutableName)
            .SetDescription(Constants.CliDescription)
            .AddCommandsFrom(typeof(Verbosity).Assembly)
            .SetVersion(Constants.CliVersion)
            .UseConsole(console)
            .Build();

    /// <summary>
    /// Gets a collection of output segments that are expected when the default help menu text is displayed.
    /// </summary>
    /// <remarks>
    /// We do not care to check for exact accuracy on the help menu output, so these segments
    /// check for a handful of known fields to be confident that we got our expected output.
    /// </remarks>
    /// <returns>A collection of expected output segments.</returns>
    internal static string[] GetExpectedDefaultHelpMenuOutputSegments() =>
        new[]
        {
            Constants.CliTitle,
            Constants.CliVersion,
            Constants.CliDescription,
            "USAGE",
            $"{Constants.CliExecutableName}",
            "OPTIONS",
            "-h|--help",
            "Shows help text.",
            "--version",
            "Shows version information.",
            "COMMANDS",
            Style.Constants.FormatCommand,
            Style.Constants.VerifyCommand,
        };

    /// <summary>
    /// Gets a snippet of expected output when run with a quiet verbosity.
    /// </summary>
    /// <remarks>
    /// We do not care to check for every instance of quiet output, so we use this segment alone to
    /// check that it appears in the output to be confident that others like it also appear and our
    /// output verbosity mechanism works as expected.
    /// </remarks>
    /// <returns>An expected quiet verbosity output segment.</returns>
    internal static string GetExpectedQuietOutputSegment() =>
        "Preparing the base system upgrade files.";

    /// <summary>
    /// Gets a snippet of expected output when run with a normal verbosity or below.
    /// </summary>
    /// <remarks>
    /// We do not care to check for every instance of normal output, so we use this segment alone to
    /// check that it appears in the output to be confident that others like it also appear and our
    /// output verbosity mechanism works as expected.
    /// </remarks>
    /// <returns>An expected normal verbosity output segment.</returns>
    internal static string GetExpectedNormalOutputSegment() =>
        "Starting the SEL UTM upgrade preparation process.";

    /// <summary>
    /// Gets a snippet of expected output when run with a verbose verbosity or below.
    /// </summary>
    /// <remarks>
    /// We do not care to check for every instance of verbose output, so we use this segment alone to
    /// check that it appears in the output to be confident that others like it also appear and our
    /// output verbosity mechanism works as expected.
    /// </remarks>
    /// <returns>An expected verbose verbosity output segment.</returns>
    internal static string GetExpectedVerboseOutputSegment() => "Downloading '";

    /// <summary>
    /// Shorthand newline specifcific to the OS running tests.
    /// </summary>
    internal static readonly string NL = Environment.NewLine;
}
