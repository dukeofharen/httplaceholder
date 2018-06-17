namespace Budgetkar.Services
{
   public interface IYamlService
   {
      TObject Parse<TObject>(string input);
   }
}
