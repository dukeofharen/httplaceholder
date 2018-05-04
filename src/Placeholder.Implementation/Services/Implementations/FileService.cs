using System;
using System.IO;

namespace Placeholder.Implementation.Services.Implementations
{
    internal class FileService : IFileService
    {
       public string ReadAllText(string path)
       {
          return File.ReadAllText(path);
       }

       public bool FileExists(string path)
       {
          return File.Exists(path);
       }

       public DateTime GetModicationDateTime(string path)
       {
          return File.GetLastWriteTimeUtc(path);
       }
    }
}
