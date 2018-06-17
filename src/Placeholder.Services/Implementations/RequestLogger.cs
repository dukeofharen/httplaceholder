using System.Text;

namespace Budgetkar.Services.Implementations
{
   internal class RequestLogger : IRequestLogger
   {
      private readonly StringBuilder _stringBuilder;

      public RequestLogger()
      {
         _stringBuilder = new StringBuilder();
      }

      public void Log(string message)
      {
         _stringBuilder.AppendLine(message);
      }

      public string FullMessage => _stringBuilder.ToString();
   }
}
