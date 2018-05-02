using System.Threading.Tasks;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation
{
   public interface IConditionChecker
   {
      Task<(ConditionCheckResultType, string)> ValidateAsync();
   }
}
