namespace HttPlaceholder.Client.Dto.Stubs
{
    /// <summary>
    /// A model for storing all information about a stub.
    /// </summary>
    public class StubDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        public StubConditionsDto Conditions { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public StubResponseDto Response { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether this stub is enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the scenario the stub is executed under.
        /// </summary>
        public string Scenario { get; set; }
    }
}
