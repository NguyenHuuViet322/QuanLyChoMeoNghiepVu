
namespace DAS.FileApi.Models
{
    public class ModelFile
    {
        public string Message { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Uploaded { get; set; }
        public long ContentLength { get; set; }
        public bool Flag { get; set; }
    }
}
