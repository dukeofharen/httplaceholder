namespace HttPlaceholder.Common
{
    public interface IAssemblyService
    {
        string GetEntryAssemblyRootPath();

        string GetExecutingAssemblyRootPath();

        string GetAssemblyVersion();

        string GetCallingAssemblyRootPath();
    }
}
