using System;

namespace Placeholder.Implementation.Services
{
   public interface IFileService
   {
      string ReadAllText(string path);

      bool FileExists(string path);

      DateTime GetModicationDateTime(string path);

      string GetCurrentDirectory();

      string[] GetFiles(string path, string searchPattern);
   }
}
