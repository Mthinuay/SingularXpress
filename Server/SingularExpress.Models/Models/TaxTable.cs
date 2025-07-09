using System;

namespace SingularExpress.Models.Models
{
    public class TaxTable
    {
        public int TaxTableId { get; set; }
        public string Year { get; set; } = string.Empty; // Changed to string to match the upload request
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int? UploadedByUserId { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}