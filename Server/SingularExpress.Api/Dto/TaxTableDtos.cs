using Microsoft.AspNetCore.Http;
using System;

namespace SingularExpress.Api.Dtos
{
    public class TaxTableDto
    {
        public int TaxTableId { get; set; }
        public string Year { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int UploadedByUserId { get; set; }
        public DateTime? UploadedDate { get; set; }
    }

    public class TaxTableUploadRequestDto
    {
        public string Year { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class TaxTableUploadResponseDto
    {
        public int TaxTableId { get; set; }
        public string Year { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public int EntryCount { get; set; }
    }

    public class TaxTableHistoryDto
    {
        public int TaxTableId { get; set; }
        public string Year { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        public int UploadedByUserId { get; set; }
    }
}