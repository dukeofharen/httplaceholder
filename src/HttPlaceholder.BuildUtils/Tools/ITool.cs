using System.Threading.Tasks;

namespace HttPlaceholder.SwaggerGenerator.Tools
{
    public interface ITool
    {
        string Key { get; }

        Task ExecuteAsync(string[] args);
    }
}
