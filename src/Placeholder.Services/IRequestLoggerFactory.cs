namespace Budgetkar.Services
{
   public interface IRequestLoggerFactory
   {
      IRequestLogger GetRequestLogger();
   }
}
