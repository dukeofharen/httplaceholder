using System;

namespace HttPlaceholder.Services
{
   public interface IFileService
   {
      string ReadAllText(string path);

      byte[] ReadAllBytes(string path);

      bool FileExists(string path);

      bool DirectoryExists(string path);

      void CreateDirectory(string path);

      DateTime GetModicationDateTime(string path);

      string GetCurrentDirectory();

      string[] GetFiles(string path, string searchPattern);

      string GetDirectoryPath(string filePath);

      bool IsDirectory(string path);

      void DeleteFile(string path);

      void WriteAllText(string path, string text);
   }
}
