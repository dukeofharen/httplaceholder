namespace Placeholder.Implementation.Services
{
   public interface IYamlService
   {
      TObject Parse<TObject>(string input);
   }
}
