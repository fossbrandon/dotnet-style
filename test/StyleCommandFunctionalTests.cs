using System.Runtime.InteropServices;
using CliFx;
using CliFx.Infrastructure;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Style.Tests.Utilities;

namespace Style.Tests;

/// <summary>
/// Provides functional tests for the default style command.
/// </summary>
public class StyleCommandFunctionalTests
{
    private static readonly string nl = TestHelpers.NL;

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public async Task StyleCommand_shows_help_menu_given_help_option(string input)
    {
        // Arrange.
        var expectedStdOutputSegments = TestHelpers.GetExpectedDefaultHelpMenuOutputSegments();

        using var console = new FakeInMemoryConsole();

        var sut = TestHelpers.BuildStyleTestApp(console);
        var args = new[] { input };

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
    public async Task StyleCommand_shows_help_menu_given_no_options()
    {
        // Arrange.
        var expectedStdOutputSegments = TestHelpers.GetExpectedDefaultHelpMenuOutputSegments();

        using var console = new FakeInMemoryConsole();

        var sut = TestHelpers.BuildStyleTestApp(console);
        var args = Array.Empty<string>();

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
    public async Task StyleCommand_shows_tool_version_given_version_option()
    {
        // Arrange.
        var expectedOutput = $"{Constants.CliVersion}{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { "--version" };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(0);
        stdOut.Should().Be(expectedOutput);
        stdError.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task StyleCommand_returns_error_given_invalid_option()
    {
        // Arrange.
        const string invalidOption = "--invalid-option";

        var expectedStdOutputSegments = TestHelpers.GetExpectedDefaultHelpMenuOutputSegments();
        var expectedErrorSegments = $"Unrecognized option(s):{nl}{invalidOption}{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { invalidOption };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedErrorSegments);
    }

    [Fact]
    public async Task StyleCommand_returns_error_given_invalid_parameter()
    {
        // Arrange.
        const string invalidArgument = "invalid-parameter";

        var expectedStdOutputSegments = TestHelpers.GetExpectedDefaultHelpMenuOutputSegments();
        var expectedErrorSegments = $"Unexpected parameter(s):{nl}<{invalidArgument}>{nl}";

        using var console = new FakeInMemoryConsole();
        var sut = TestHelpers.BuildStyleTestApp(console);

        var args = new[] { invalidArgument };

        // Act.
        var exitCode = await sut.RunAsync(args);
        var stdOut = console.ReadOutputString();
        var stdError = console.ReadErrorString();

        // Assert.
        exitCode.Should().Be(1);
        stdOut.Should().ContainAll(expectedStdOutputSegments);
        stdError.Should().Be(expectedErrorSegments);
    }
}
