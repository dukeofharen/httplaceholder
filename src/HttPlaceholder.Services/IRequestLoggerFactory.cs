namespace HttPlaceholder.Services
{
    public interface IRequestLoggerFactory
    {
        IRequestLogger GetRequestLogger();
    }
}