namespace Style;

/// <summary>
/// A collection of commonly used, immutable values.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The format command name.
    /// </summary>
    public const string FormatCommand = "format";

    /// <summary>
    /// The verify command name.
    /// </summary>
    public const string VerifyCommand = "verify";

    /// <summary>
    /// The path CLI option.
    /// </summary>
    public const string PathOption = "path";

    /// <summary>
    /// The verbosity CLI option.
    /// </summary>
    public const string VerbosityOption = "verbosity";

    /// <summary>
    /// Format using CSharpier CLI option
    /// </summary>
    public const string CsharpierOption = "csharpier";

    /// <summary>
    /// Format using dotnet format style CLI option.
    /// </summary>
    public const string StyleOption = "style";

    /// <summary>
    /// Format using dotnet format analyzers CLI option.
    /// </summary>
    public const string AnalyzersOption = "analyzers";

    /// <summary>
    /// Form using dotnet format whitespace CLI option.
    /// </summary>
    public const string WhitespaceOption = "whitespace";

    /// <summary>
    /// The .NET CLI command.
    /// </summary>
    public const string DotnetCli = "dotnet";

    /// <summary>
    /// Specify a higher verbosity option message.
    /// </summary>
    /// <remarks>The space is intentional as it will get appended to a sentence.</remarks>
    public const string UseHigherVerbosityMessage =
        " For more information, try running the command "
        + "again and specify a higher verbosity option.";
}
