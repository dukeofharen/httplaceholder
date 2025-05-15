namespace HttPlaceholder.Domain;

/// <summary>
///     A model that is used for displaying what types of variable handlers are available.
/// </summary>
public class VariableHandlerModel
{
    /// <summary>
    ///     Constructs a <see cref="VariableHandlerModel" /> instance.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="fullName">The full name.</param>
    /// <param name="examples">The examples.</param>
    /// <param name="description">The description.</param>
    public VariableHandlerModel(string name, string fullName, string[] examples, string description)
    {
        Name = name;
        FullName = fullName;
        Examples = examples;
        Description = description;
    }

    /// <summary>
    ///     Gets or sets the name of the variable handler.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets or sets the full name of the variable handler.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    ///     Gets or sets a short instruction on how to use the variable handler.
    /// </summary>
    public string Example => string.Join(' ', Examples);

    /// <summary>
    ///     Gets or sets one or more examples.
    /// </summary>
    public string[] Examples { get; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    public string Description { get; }
}
