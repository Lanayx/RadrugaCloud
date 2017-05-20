namespace Core.CommonModels
{
    using System.IO;

    public class FileData
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }

        public string ContentType { get; set; }
    }
}
