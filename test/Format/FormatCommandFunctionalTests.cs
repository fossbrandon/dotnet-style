using CliFx.Infrastructure;
using FluentAssertions;
using Style.Format;
using Style.Tests.Utilities;

namespace Style.Tests.Format;

/// <summary>
/// Provides functional tests for <see cref="FormatCommand"/>.
/// </summary>
public class FormatCommandFunctionalTests
{
    private static readonly string nl = TestHelpers.NL;
    private const string Command = Style.Constants.FormatCommand;

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public async Task FormatCommand_shows_help_menu_given_help_option(string input)
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();

        using var console = new FakeInMemoryConsole();

        var sut = TestHelpers.BuildStyleTestApp(console);
        var args = new[] { Command, input };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(0);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task FormatCommand_returns_error_given_invalid_option()
    {
        // Arrange.
        const string invalidOption = "--invalid-option";

        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();
        var expectedStdError = $"Unrecognized option(s):{nl}{invalidOption}{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { Command, invalidOption };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedStdError);
    }

    [Fact]
    public async Task FormatCommand_returns_error_given_invalid_option_value()
    {
        // Arrange.
        const string validOption = $"--{Style.Constants.CsharpierOption}";
        const string invalidValue = "invalid";

        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();
        var expectedStdError =
            $"Option -c|{validOption} cannot be set from the provided argument(s):{nl}"
            + $"<{invalidValue}>{nl}"
            + $"Error: String '{invalidValue}' was not recognized as a valid Boolean.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { Command, validOption, invalidValue };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedStdError);
    }

    [Fact]
    public async Task FormatCommand_returns_error_given_invalid_parameter()
    {
        // Arrange.
        const string invalidParameter = "invalid-parameter";

        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();
        var expectedStdError = $"Unexpected parameter(s):{nl}<{invalidParameter}>{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { Command, invalidParameter };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedStdError);
    }

    [Fact]
    public async Task FormatCommand_returns_error_when_no_formatters_are_enabled()
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();
        var expectedStdError = $"You must enable at least one formatter to format code with.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[]
        {
            Command,
            $"--{Style.Constants.StyleOption}",
            "false",
            $"--{Style.Constants.AnalyzersOption}",
            "false",
            $"--{Style.Constants.WhitespaceOption}",
            "false",
            $"--{Style.Constants.CsharpierOption}",
            "false",
        };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedStdError);
    }

    [Fact]
    public async Task FormatCommand_returns_error_when_multiple_whitespace_formatters_are_enabled()
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedFormatHelpMenuOutputSegments();
        var expectedStdError =
            "You may only enable one whitespace formatter to format code with "
            + $"by specifying either the '--{Style.Constants.CsharpierOption}' option or the "
            + $"'--{Style.Constants.WhitespaceOption}' option to avoid potential conflicts.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[]
        {
            Command,
            $"--{Style.Constants.StyleOption}",
            "false",
            $"--{Style.Constants.AnalyzersOption}",
            "false",
            $"--{Style.Constants.WhitespaceOption}",
            "true",
            $"--{Style.Constants.CsharpierOption}",
            "true",
        };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedStdError);
    }

    internal static string[] GetExpectedFormatHelpMenuOutputSegments() =>
        new[]
        {
            Constants.CliTitle,
            Constants.CliVersion,
            Constants.CliDescription,
            "USAGE",
            $"{Constants.CliExecutableName}",
            Style.Constants.FormatCommand,
            "DESCRIPTION",
            "OPTIONS",
            "-v|--verbosity",
            "The output verbosity level. Choices: \"Quiet\", \"Normal\", \"Verbose\". Default: \"Normal\".",
            "-h|--help",
            "Shows help text.",
        };
}
