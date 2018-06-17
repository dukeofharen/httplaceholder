using System;
using System.IO;

namespace Budgetkar.Services.Implementations
{
    internal class FileService : IFileService
    {
       public string ReadAllText(string path)
       {
          return File.ReadAllText(path);
       }

       public byte[] ReadAllBytes(string path)
       {
          return File.ReadAllBytes(path);
       }

       public bool FileExists(string path)
       {
          return File.Exists(path);
       }

       public DateTime GetModicationDateTime(string path)
       {
          return File.GetLastWriteTimeUtc(path);
       }

       public string GetCurrentDirectory()
       {
          return Directory.GetCurrentDirectory();
       }

       public string[] GetFiles(string path, string searchPattern)
       {
          return Directory.GetFiles(path, searchPattern);
       }

       public string GetDirectoryPath(string filePath)
       {
          return Path.GetDirectoryName(filePath);
       }
    }
}
