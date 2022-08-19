using System;
using Bogus;

namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
/// A model that contains metadata for a fake data generator.
/// </summary>
public class FakeDataGeneratorModel
{
    /// <summary>
    /// Constructs a <see cref="FakeDataGeneratorModel"/> instance.
    /// </summary>
    public FakeDataGeneratorModel(string name, string formatting, Func<string, string> formatterFunction)
    {
        Name = name;
        Formatting = formatting;
        FormatterFunction = formatterFunction;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the formatting.
    /// </summary>
    public string Formatting { get; }

    /// <summary>
    /// Gets or sets the formatter function.
    /// </summary>
    public Func<Faker, string, string> FormatterFunction { get; }
}
