namespace HttPlaceholder.Services
{
   public interface IYamlService
   {
      TObject Parse<TObject>(string input);
   }
}
