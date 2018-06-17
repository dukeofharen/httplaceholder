namespace Placeholder.Services
{
   public interface IYamlService
   {
      TObject Parse<TObject>(string input);
   }
}
