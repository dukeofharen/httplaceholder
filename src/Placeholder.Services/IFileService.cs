using System;

namespace Budgetkar.Services
{
   public interface IFileService
   {
      string ReadAllText(string path);

      byte[] ReadAllBytes(string path);

      bool FileExists(string path);

      DateTime GetModicationDateTime(string path);

      string GetCurrentDirectory();

      string[] GetFiles(string path, string searchPattern);

      string GetDirectoryPath(string filePath);
   }
}
