namespace Budgetkar.Services
{
    public interface IRequestLogger
    {
       void Log(string message);

       string FullMessage { get; }
    }
}
