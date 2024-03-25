namespace ESD.Domain.Models.CustomModels
{
    public class EmailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Trace { get; set; }
    }
}
