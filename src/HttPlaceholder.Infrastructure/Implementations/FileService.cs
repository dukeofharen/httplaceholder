using System;
using System.IO;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class FileService : IFileService
    {
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllBytes(string path, byte[] contents) => File.WriteAllBytes(path, contents);

        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

        public bool FileExists(string path) => File.Exists(path);

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public string GetTempPath() => Path.GetTempPath();

        public void DeleteFile(string path) => File.Delete(path);

        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

        public bool IsDirectory(string path) => (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;

        public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public DateTime GetModicationDateTime(string path) => File.GetLastWriteTimeUtc(path);
    }
}
