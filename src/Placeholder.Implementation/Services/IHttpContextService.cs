namespace Placeholder.Implementation.Services
{
   public interface IHttpContextService
   {
      string Method { get; }

      string Path { get; }
   }
}
