namespace HttPlaceholder.Domain;

/// <summary>
/// A model that is used for displaying what types of variable handlers are available.
/// </summary>
public class VariableHandlerModel
{
    public VariableHandlerModel(string name, string fullName, string example)
    {
        Name = name;
        FullName = fullName;
        Example = example;
    }

    /// <summary>
    /// Gets or sets the name of the variable handler.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the full name of the variable handler.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Gets or sets a short instruction on how to use the variable handler.
    /// </summary>
    public string Example { get; }
}