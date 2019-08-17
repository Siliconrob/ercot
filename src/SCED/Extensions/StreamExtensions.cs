using System.IO;
using System.Threading.Tasks;

namespace SCED.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadStreamAsync(this Stream input)
        {
            using (var memReader = new StreamReader(input))
            {
                return await memReader.ReadToEndAsync();
            }
        }
    }
}