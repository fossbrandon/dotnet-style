using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CliFx;
using CliFx.Infrastructure;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Style.Tests.Utilities;
using Style.Verify;

namespace Style.Tests.Verify;

/// <summary>
/// Provides functional tests for <see cref="VerifyCommand"/>.
/// </summary>
public class VerifyCommandFunctionalTests
{
    private static readonly string nl = TestHelpers.NL;
    private const string command = Style.Constants.VerifyCommand;

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public async Task VerifyCommand_shows_help_menu_given_help_option(string input)
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();

        using var console = new FakeInMemoryConsole();

        var sut = TestHelpers.BuildStyleTestApp(console);
        var args = new[] { command, input };

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
    public async Task VerifyCommand_returns_error_given_invalid_option()
    {
        // Arrange.
        const string invalidOption = "--invalid-option";

        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();
        var expectedStdError = $"Unrecognized option(s):{nl}{invalidOption}{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { command, invalidOption };

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
    public async Task VerifyCommand_returns_error_given_invalid_option_value()
    {
        // Arrange.
        const string validOption = $"--{Style.Constants.CsharpierOption}";
        const string invalidValue = "invalid";

        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();
        var expectedStdError = $"Option -c|{validOption} cannot be set from the provided argument(s):{nl}" +
            $"<{invalidValue}>{nl}" +
            $"Error: String '{invalidValue}' was not recognized as a valid Boolean.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { command, validOption, invalidValue };

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
    public async Task VerifyCommand_returns_error_given_invalid_parameter()
    {
        // Arrange.
        const string invalidParameter = "invalid-parameter";

        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();
        var expectedStdError = $"Unexpected parameter(s):{nl}<{invalidParameter}>{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { command, invalidParameter };

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
    public async Task VerifyCommand_returns_error_when_no_formatters_are_enabled()
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();
        var expectedStdError = $"You must enable at least one formatter to verify the code style with.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[]
        {
            command,
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
    public async Task VerifyCommand_returns_error_when_multiple_whitespace_formatters_are_enabled()
    {
        // Arrange.
        var expectedStdOutputSegments = GetExpectedVerifyHelpMenuOutputSegments();
        var expectedStdError = "You may only enable one whitespace formatter to verify code style " +
            $"compliance with by specifying either the '--{Style.Constants.CsharpierOption}' option or the " +
            $"'--{Style.Constants.WhitespaceOption}' option to avoid potential conflicts.{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[]
        {
            command,
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

    internal static string[] GetExpectedVerifyHelpMenuOutputSegments() =>
       new[]
       {
           Constants.CliTitle,
           Constants.CliVersion,
           Constants.CliDescription,
           "USAGE",
           $"{Constants.CliExecutableName}",
           Style.Constants.VerifyCommand,
           "DESCRIPTION",
           "OPTIONS",
           "-v|--verbosity",
           "The output verbosity level. Choices: \"Quiet\", \"Normal\", \"Verbose\". Default: \"Normal\".",
           "-h|--help",
           "Shows help text.",
       };
}
