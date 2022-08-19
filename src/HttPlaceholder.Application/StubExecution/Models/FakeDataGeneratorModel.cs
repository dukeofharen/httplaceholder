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
    public FakeDataGeneratorModel(string name, string formatting, Func<Faker, string, string, string> formatterFunction)
    {
        Name = name;
        Formatting = formatting;
        FormatterFunction = formatterFunction;
    }

    /// <summary>
    /// Constructs a <see cref="FakeDataGeneratorModel"/> instance.
    /// </summary>
    public FakeDataGeneratorModel(string name, Func<Faker, string, string, string> formatterFunction)
    {
        Name = name;
        FormatterFunction = formatterFunction;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the formatting.
    /// </summary>
    public string Formatting { get; } = string.Empty;

    /// <summary>
    /// Gets or sets the formatter function.
    /// First type argument is the faker, second type argument is the formatting, third type argument is the locale and the output type is the result.
    /// </summary>
    public Func<Faker, string, string, string> FormatterFunction { get; }
}
