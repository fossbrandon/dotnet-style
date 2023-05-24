namespace Style;

/// <summary>
/// The available verbosity levels for CLI output.
/// </summary>
public enum Verbosity
{
    /// <summary>
    /// Supress all output.
    /// </summary>
    /// <remarks>
    /// The exit code will provide information on whether the command passed.
    /// </remarks>
    Quiet = 0,

    /// <summary>
    /// Output standard progress updates.
    /// </summary>
    /// <remarks>
    /// Suppresses debug style information that is not vital to understand program progress.
    /// </remarks>
    Normal = 1,

    /// <summary>
    /// Output all available information.
    /// </summary>
    /// <remarks>
    /// Provides debug level information that may help diagnose issues.
    /// </remarks>
    Verbose = 2,
}
