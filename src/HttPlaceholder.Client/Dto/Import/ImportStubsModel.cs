namespace HttPlaceholder.Client.Dto.Import;

/// <summary>
///     A model for batch importing stubs in HttPlaceholder.
/// </summary>
public class ImportStubsModel
{
    /// <summary>
    /// Constructs an <see cref="ImportStubsModel"/>.
    /// </summary>
    public ImportStubsModel()
    {
    }

    /// <summary>
    /// Constructs an <see cref="ImportStubsModel"/>.
    /// </summary>
    public ImportStubsModel(string input)
    {
        Input = input;
    }

    /// <summary>
    ///     The stub import input.
    /// </summary>
    public string Input { get; set; }

    /// <summary>
    ///     Whether to add the stub to the data source. If set to false, the stub is only returned
    ///     but not added.
    /// </summary>
    public bool DoNotCreateStub { get; set; }

    /// <summary>
    ///     The tenant (category) the stubs should be added under. If no tenant is provided, a tenant name
    ///     will be generated.
    /// </summary>
    public string Tenant { get; set; }

    /// <summary>A piece of text that will be prefixed before the stub ID.</summary>
    public string StubIdPrefix { get; set; }
}
