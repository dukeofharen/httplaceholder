using Microsoft.Extensions.Options;

namespace HttPlaceholder.TestUtilities.Options
{
    public class OptionsMock<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        public OptionsMock(TOptions instance)
        {
            Value = instance;
        }

        public TOptions Value { get; }
    }
}
