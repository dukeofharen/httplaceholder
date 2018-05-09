namespace Placeholder.Implementation.Services
{
    public interface IRequestLogger
    {
       void Log(string message);

       string FullMessage { get; }
    }
}
