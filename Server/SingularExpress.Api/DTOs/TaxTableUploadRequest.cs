using System;

namespace SingularExpress.Dto
{
    public class TaxTable
    {
        public int TaxTableId { get; set; }
        public int Year { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int? UploadedByUserId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}