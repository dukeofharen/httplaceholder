using System.Security.Cryptography;
using System.Text;

namespace HttPlaceholder.Services.Implementations
{
    internal class HashingService : IHashingService
    {
        public string GetMd5String(string input)
        {
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
