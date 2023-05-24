using CliFx.Infrastructure;
using CliWrap.Buffered;
using FluentAssertions;
using Style.Extensions;

namespace Style.Tests.Extensions;

/// <summary>
/// Provides unit tests for the <see cref="Style.Extensions.ConsoleExtensions"/> class.
/// </summary>
public class ConsoleExtensionsUnitTests
{
    private readonly string nl = Environment.NewLine;
    private readonly List<Verbosity> quietAndGreaterVerbosities =
        new() { Verbosity.Quiet, Verbosity.Normal, Verbosity.Verbose };
    private readonly List<Verbosity> normalAndGreaterVerbosities =
        new() { Verbosity.Normal, Verbosity.Verbose };
    private readonly List<Verbosity> verboseAndGreaterVerbosities = new() { Verbosity.Verbose };

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("    ", "    ")]
    [InlineData("\n", "\n")]
    [InlineData("\t", $"\t")]
    [InlineData("This should appear in the console", "This should appear in the console")]
    [InlineData(
        "This multiline message\nshould appear in the console",
        "This multiline message\nshould appear in the console"
    )]
    public async Task WriteQuietLineAsync_writes_to_the_console_given_a_message_and_a_verbosity_of_quiet_or_higher(
        string? message,
        string expected
    )
    {
        foreach (var verbosity in quietAndGreaterVerbosities)
        {
            // Arrange.
            using var console = new FakeInMemoryConsole();
            var sanitizedExpected = $"{expected}{nl}";

            // Act.
            await console.Output.WriteQuietLineAsync(message, verbosity);
            var output = console.ReadOutputString();

            // Assert.
            output.Should().Be(sanitizedExpected);
        }
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("    ", "    ")]
    [InlineData("\n", "\n")]
    [InlineData("\t", $"\t")]
    [InlineData("This should appear in the console", "This should appear in the console")]
    [InlineData(
        "This multiline message\nshould appear in the console",
        "This multiline message\nshould appear in the console"
    )]
    public async Task WriteNormalLineAsync_writes_to_the_console_given_a_message_and_a_verbosity_of_normal_or_higher(
        string? message,
        string expected
    )
    {
        foreach (var verbosity in normalAndGreaterVerbosities)
        {
            // Arrange.
            using var console = new FakeInMemoryConsole();
            var sanitizedExpected = $"{expected}{nl}";

            // Act.
            await console.Output.WriteNormalLineAsync(message, verbosity);
            var output = console.ReadOutputString();

            // Assert.
            output.Should().Be(sanitizedExpected);
        }
    }

    [Theory]
    [InlineData(Verbosity.Quiet)]
    public async Task WriteNormalLineAsync_does_not_write_to_the_console_given_a_message_and_a_verbosity_lower_than_normal(
        Verbosity verbosity
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        const string message = "This should not appear in the console";

        // Act.
        await console.Output.WriteNormalLineAsync(message, verbosity);
        var output = console.ReadOutputString();

        // Assert.
        output.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("    ", "    ")]
    [InlineData("\n", "\n")]
    [InlineData("\t", $"\t")]
    [InlineData("This should appear in the console", "This should appear in the console")]
    [InlineData(
        "This multiline message\nshould appear in the console",
        "This multiline message\nshould appear in the console"
    )]
    public async Task WriteVerboseLineAsync_writes_to_the_console_given_a_message_and_a_verbosity_of_verbose_or_higher(
        string? message,
        string expected
    )
    {
        foreach (var verbosity in verboseAndGreaterVerbosities)
        {
            // Arrange.
            using var console = new FakeInMemoryConsole();
            var sanitizedExpected = $"{expected}{nl}";

            // Act.
            await console.Output.WriteVerboseLineAsync(message, verbosity);
            var output = console.ReadOutputString();

            // Assert.
            output.Should().Be(sanitizedExpected);
        }
    }

    [Theory]
    [InlineData(Verbosity.Quiet)]
    [InlineData(Verbosity.Normal)]
    public async Task WriteVerboseLineAsync_does_not_write_to_the_console_given_a_message_and_a_verbosity_lower_than_verbose(
        Verbosity verbosity
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        const string message = "This should not appear in the console";

        // Act.
        await console.Output.WriteVerboseLineAsync(message, verbosity);
        var output = console.ReadOutputString();

        // Assert.
        output.Should().BeEmpty();
    }

    [Theory]
    [InlineData(
        Style.Constants.DotnetCli,
        Style.Constants.CsharpierOption,
        $"Running the command '{Style.Constants.DotnetCli} {Style.Constants.CsharpierOption}'"
    )]
    [InlineData("command", "", $"Running the command 'command'")]
    [InlineData(" command ", " arguments ", $"Running the command 'command arguments'")]
    [InlineData("command", null, $"Running the command 'command'")]
    [InlineData("command", "    ", $"Running the command 'command'")]
    [InlineData("command", "\t", $"Running the command 'command'")]
    [InlineData("command", "\n", $"Running the command 'command'")]
    [InlineData(
        "command",
        "arguments --path-option \"/etc/example\"",
        $"Running the command 'command arguments --path-option \"/etc/example\"'"
    )]
    [InlineData(
        "command",
        "--multiline\narguments",
        $"Running the command 'command --multiline\narguments'"
    )]
    public async Task WriteCliCommandToRunAsync_writes_to_the_console_given_a_message_and_a_verbosity_of_normal_or_higher(
        string command,
        string arguments,
        string expected
    )
    {
        foreach (var verbosity in normalAndGreaterVerbosities)
        {
            // Arrange.
            using var console = new FakeInMemoryConsole();
            var sanitizedExpected = $"{expected}{nl}";

            // Act.
            await console.WriteCliCommandToRunAsync(command, arguments, verbosity);
            var output = console.ReadOutputString();

            // Assert.
            output.Should().Be(sanitizedExpected);
        }
    }

    [Theory]
    [InlineData(Verbosity.Quiet)]
    public async Task WriteCliCommandToRunAsync_does_not_write_to_the_console_given_a_message_and_a_verbosity_lower_than_normal(
        Verbosity verbosity
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        const string command = "command";
        const string arguments = "arguments";

        // Act.
        await console.WriteCliCommandToRunAsync(command, arguments, verbosity);
        var output = console.ReadOutputString();

        // Assert.
        output.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task WriteCliCommandToRunAsync_throws_exception_given_empty_command(
        string? command
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        const string arguments = "arguments";

        // Act.
        Func<Task> actAsync = async () =>
            await console.WriteCliCommandToRunAsync(command!, arguments, Verbosity.Normal);

        // Assert.
        await actAsync
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithMessage("The parameter must be a non-empty value (Parameter 'command')");
    }

    [Theory]
    [InlineData("Should show up")]
    public async Task WriteCliCommandStandardOutputAsync_writes_to_the_console_given_the_command_result_with_non_empty_standard_output_and_a_verbosity_of_verbose_or_higher(
        string standardOuptut
    )
    {
        foreach (var verbosity in verboseAndGreaterVerbosities)
        {
            // Arrange.
            using var console = new FakeInMemoryConsole();
            var result = new BufferedCommandResult(
                0,
                new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 00)),
                new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 05)),
                standardOuptut,
                ""
            );
            var sanitizedExpected = $"Command Output:{nl}{nl}{standardOuptut}{nl}{nl}";

            // Act.
            await console.WriteCliCommandStandardOutputAsync(result, verbosity);
            var output = console.ReadOutputString();

            // Assert.
            output.Should().Be(sanitizedExpected);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task WriteCliCommandStandardOutputAsync_does_not_write_to_the_console_given_result_with_empty_standard_output(
        string? emptyStandardOutput
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        var result = new BufferedCommandResult(
            0,
            new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 00)),
            new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 05)),
            emptyStandardOutput!,
            ""
        );

        // Act.
        await console.WriteCliCommandStandardOutputAsync(result, Verbosity.Verbose);
        var output = console.ReadOutputString();

        // Assert.
        output.Should().BeEmpty();
    }

    [Theory]
    [InlineData(Verbosity.Quiet)]
    public async Task WriteCliCommandStandardOutputAsync_does_not_write_to_the_console_given_the_command_result_and_a_verbosity_lower_than_normal(
        Verbosity verbosity
    )
    {
        // Arrange.
        using var console = new FakeInMemoryConsole();
        var result = new BufferedCommandResult(
            0,
            new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 00)),
            new DateTimeOffset(new DateTime(2023, 05, 04, 01, 00, 05)),
            "Should not show up",
            "Fake error message"
        );

        // Act.
        await console.WriteCliCommandStandardOutputAsync(result, verbosity);
        var output = console.ReadOutputString();

        // Assert.
        output.Should().BeEmpty();
    }
}
