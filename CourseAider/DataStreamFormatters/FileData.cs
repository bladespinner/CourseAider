using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CourseAider.DataStreamFormatters
{
    /// <summary>
    /// Class to store attached file info
    /// </summary>
    public class FileData
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }

        public long Size { get { return (Data != null ? Data.LongLength : 0L); } }

        /// <summary>
        /// Create a FileData from HttpContent 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<FileData> ReadFile(HttpContent file)
        {
            var data = await file.ReadAsByteArrayAsync();
            var result = new FileData()
            {
                FileName = FixFilename(file.Headers.ContentDisposition.FileName),
                ContentType = file.Headers.ContentType.ToString(),
                Data = data
            };
            return result;
        }

        /// <summary>
        /// Amend filenames to remove surrounding quotes and remove path from IE
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static string FixFilename(string original)
        {
            var result = original.Trim();
            // remove leading and trailing quotes
            if (result.StartsWith("\""))
                result = result.TrimStart('"').TrimEnd('"');
            // remove full path versions
            if (result.Contains("\\"))
                // parse out path
                result = new System.IO.FileInfo(result).Name;

            return result;
        }
    }
}