namespace Style.Attributes;

/// <summary>
/// Represents a CLI formatter option.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormatterOptionAttribute : Attribute
{
    /// <summary>
    /// Gets or initializes an identifier for this CLI formatter option.
    /// </summary>
    /// <remarks>
    /// This value should be unique so that other attributes can reference the expected attribute.
    /// </remarks>
    public string Identifier { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="FormatterOptionAttribute"/>.
    /// </summary>
    /// <param name="identifier">A unique identifier for this CLI formatter option.</param>
    public FormatterOptionAttribute(string identifier) => Identifier = identifier;
}
