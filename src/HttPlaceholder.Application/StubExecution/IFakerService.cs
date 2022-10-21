using HttPlaceholder.Application.StubExecution.Models;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to generate fake data.
/// </summary>
public interface IFakerService
{
    /// <summary>
    ///     Returns a list of available fake data generator names.
    /// </summary>
    /// <returns>The list of fake data generator names.</returns>
    FakeDataGeneratorModel[] GetGenerators();

    /// <summary>
    ///     Returns a list of available Faker locales.
    /// </summary>
    /// <returns>The list of locales.</returns>
    string[] GetLocales();

    /// <summary>
    ///     Generates fake data.
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="locale"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    string GenerateFakeData(string generator, string locale, string format);
}
