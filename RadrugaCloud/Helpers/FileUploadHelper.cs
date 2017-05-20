using Core.CommonModels;
using RadrugaCloud.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace RadrugaCloud.Helpers
{
    using System.IO;
    using System.Text;

    using Core.Enums;

    public class FileUploadHelper
    {
        public static async Task<MultipartData> GetMultipartContent(HttpRequestMessage request, string textDataHeader)
        {
            var streamProvider = await request.Content.ReadAsMultipartAsync();
            var result = new MultipartData();
            if (streamProvider.Contents.Count == 0)
            {
                result.ErrorType = MultipartDataError.FileDataEmpty;
                return result;
            }

            if (streamProvider.Contents.Count == 1) // from simulator
            {
                IEnumerable<string> textData;
                if (!request.Headers.TryGetValues(textDataHeader, out textData))
                {
                    result.ErrorType = MultipartDataError.TextDataEmpty;
                    return result;
                }

                var text = textData.First();
                if (string.IsNullOrEmpty(text))
                {
                    result.ErrorType = MultipartDataError.TextDataEmpty;
                    return result;
                }
                result.TextData = text;
            }
            else // from devices, count == 2
            {
                var nameContent =
                    streamProvider.Contents.FirstOrDefault(
                        cnt => cnt.Headers != null && cnt.Headers.ContentDisposition != null && cnt.Headers.ContentDisposition.Name == "\"" + textDataHeader + "\"");
                if (nameContent == null)
                {
                    result.ErrorType = MultipartDataError.TextDataEmpty;
                    return result;
                }
                result.TextData = await nameContent.ReadAsStringAsync();
            }

            var fileContent = streamProvider.Contents.FirstOrDefault(cnt =>
                cnt.Headers.ContentDisposition == null || string.IsNullOrEmpty(cnt.Headers.ContentDisposition.Name) || cnt.Headers.ContentDisposition.Name == "\"\"" || // stub for windows phone emulator (possibly for windows phone real)
                cnt.Headers.ContentDisposition.Name == "\"file\"" || cnt.Headers.ContentDisposition.Name == "file");
            if (fileContent != null)
            {
                var fileData = new FileData();

                // stub for WP emulator
                if (fileContent.Headers.ContentDisposition == null)
                {
                    await FillFileDataForWP(fileContent, fileData);
                }
                else
                {
                    fileData.ContentType = fileContent.Headers.ContentType.MediaType;
                    fileData.FileName = fileContent.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    fileData.InputStream = await fileContent.ReadAsStreamAsync();
                }
                result.FileData = fileData;
                return result;
            }
            else
            {
                result.ErrorType = MultipartDataError.FileDataEmpty;
                return result;
            }
        }

        public static async Task<MultipartData> GetOneFileContent(HttpRequestMessage request)
        {
            var streamProvider = await request.Content.ReadAsMultipartAsync();
            var file = streamProvider.Contents.FirstOrDefault();
            if (file != null)
            {
                var fileData = new FileData
                {
                    ContentType = file.Headers.ContentType.MediaType,
                    FileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty),
                    InputStream = await file.ReadAsStreamAsync(),
                };
                return new MultipartData { FileData = fileData };
            }
            else
            {
                return new MultipartData { ErrorType = MultipartDataError.FileDataEmpty };
            }
        }

        private static async Task FillFileDataForWP(HttpContent fileContent, FileData fileData)
        {
            var stream = await fileContent.ReadAsStreamAsync();
            var readedContent = ReadThreeLines(stream);
            fileData.ContentType = readedContent[1].Substring(readedContent[1].IndexOf("Content-Type: ", StringComparison.Ordinal) + 14);
            var fileNameWithLastQuote = readedContent[0].Substring(readedContent[0].IndexOf("filename=\"", StringComparison.Ordinal) + 10);
            fileData.FileName = fileNameWithLastQuote.Substring(0, fileNameWithLastQuote.Length - 1);
            fileData.InputStream = stream;
        }

        private static string[] ReadThreeLines(Stream stream)
        {
            var lines = new string[2];
            const int MaxCounter = 1000;
            var newLineCounter = 0;
            var sb = new StringBuilder();
            for (var i = 0; i < MaxCounter; i++)
            {
                var readCharacter = (char)stream.ReadByte();
                if (readCharacter == '\n')
                {
                    if (newLineCounter + 1 == 3)
                    {
                        return lines;
                    }

                    lines[newLineCounter] = sb.Remove(sb.Length - 1, 1).ToString(); // removing \r (\r\n come together)
                    sb.Clear();
                    newLineCounter++;
                }
                else
                {
                    sb.Append(readCharacter);
                }
            }

            throw new Exception("WP format is different");
        }
    }
}