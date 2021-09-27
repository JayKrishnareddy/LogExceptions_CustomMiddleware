using System;

namespace LogExceptions_CustomMiddleware.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string ExceptionMessage { get; set; }
        public DateTime CretedDate { get; set; }
    }
}
