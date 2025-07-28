using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SingularExpress.Api.Dtos;
using SingularExpress.Api.Repositories;
using SingularExpress.Models;
using SingularExpress.Models.Models;

namespace SingularExpress.Api.Services
{
    public class TaxTableService : ITaxTableService
    {
        private readonly ITaxTableRepository _taxTableRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TaxTableService> _logger;

        public TaxTableService(ITaxTableRepository taxTableRepository, IWebHostEnvironment env, ILogger<TaxTableService> logger)
        {
            _taxTableRepository = taxTableRepository ?? throw new ArgumentNullException(nameof(taxTableRepository));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TaxTableDto>> GetAllTaxTablesAsync()
        {
            var taxTables = await _taxTableRepository.GetAllTaxTablesAsync();
            return taxTables.Select(t => new TaxTableDto
            {
                TaxTableId = t.TaxTableId,
                Year = t.Year,
                FileName = t.FileName,
                FileUrl = t.FileUrl,
                UploadedByUserId = t.UploadedByUserId ?? 0,
                UploadedDate = t.UploadedDate
            }).ToList();
        }

        public async Task<TaxTableDto?> GetTaxTableByIdAsync(int id)
        {
            var taxTable = await _taxTableRepository.GetTaxTableByIdAsync(id);
            if (taxTable == null)
                return null;

            return new TaxTableDto
            {
                TaxTableId = taxTable.TaxTableId,
                Year = taxTable.Year,
                FileName = taxTable.FileName,
                FileUrl = taxTable.FileUrl,
                UploadedByUserId = taxTable.UploadedByUserId ?? 0,
                UploadedDate = taxTable.UploadedDate
            };
        }

        public async Task<TaxTableDto> CreateTaxTableAsync(TaxTableDto taxTableDto)
        {
            if (string.IsNullOrEmpty(taxTableDto.Year) || !Regex.IsMatch(taxTableDto.Year, @"^\d{4}-\d{4}$"))
            {
                throw new ArgumentException("Invalid year format. Use 'YYYY-YYYY'.");
            }

            var taxTable = new TaxTable
            {
                Year = taxTableDto.Year,
                FileName = taxTableDto.FileName,
                FileUrl = taxTableDto.FileUrl,
                UploadedByUserId = taxTableDto.UploadedByUserId,
                UploadedDate = taxTableDto.UploadedDate ?? DateTime.UtcNow
            };

            await _taxTableRepository.AddTaxTableAsync(taxTable);

            if (taxTable.TaxTableId == 0)
            {
                throw new InvalidOperationException("TaxTableId was not assigned after adding the tax table.");
            }

            return new TaxTableDto
            {
                TaxTableId = taxTable.TaxTableId,
                Year = taxTable.Year,
                FileName = taxTable.FileName,
                FileUrl = taxTable.FileUrl,
                UploadedByUserId = taxTable.UploadedByUserId ?? 0,
                UploadedDate = taxTable.UploadedDate
            };
        }

        public async Task UpdateTaxTableAsync(TaxTableDto taxTableDto)
        {
            if (string.IsNullOrEmpty(taxTableDto.Year) || !Regex.IsMatch(taxTableDto.Year, @"^\d{4}-\d{4}$"))
            {
                throw new ArgumentException("Invalid year format. Use 'YYYY-YYYY'.");
            }

            var existingTaxTable = await _taxTableRepository.GetTaxTableByIdAsync(taxTableDto.TaxTableId);
            if (existingTaxTable == null)
            {
                throw new KeyNotFoundException($"Tax table with ID {taxTableDto.TaxTableId} not found.");
            }

            existingTaxTable.Year = taxTableDto.Year;
            existingTaxTable.FileName = taxTableDto.FileName;
            existingTaxTable.FileUrl = taxTableDto.FileUrl;
            existingTaxTable.UploadedByUserId = taxTableDto.UploadedByUserId;
            existingTaxTable.UploadedDate = taxTableDto.UploadedDate ?? DateTime.UtcNow;

            await _taxTableRepository.UpdateTaxTableAsync(existingTaxTable);
        }

        public async Task DeleteTaxTableAsync(int id)
        {
            var exists = await _taxTableRepository.TaxTableExistsAsync(id);
            if (!exists)
            {
                throw new KeyNotFoundException($"Tax table with ID {id} not found.");
            }
            await _taxTableRepository.DeleteTaxTableAsync(id);
        }

        public async Task<TaxTableUploadResponseDto> UploadTaxTableAsync(TaxTableUploadRequestDto request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var trimmedYear = request.Year?.Trim().Trim('"').Trim('\'');
                if (trimmedYear == null)
                {
                    throw new ArgumentException("Year is required.");
                }
                ValidateUploadRequest(trimmedYear, request.File);

                var folder = Path.Combine(_env.ContentRootPath, "Uploads", "TaxTables");
                Directory.CreateDirectory(folder);

                var extension = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folder, uniqueName);

                await SaveFileAsync(request.File, filePath);

                var taxTableEntries = await ParseExcelFileAsync(request.File, trimmedYear);
                if (!taxTableEntries.Any())
                {
                    throw new ArgumentException("No valid tax table entries found in the Excel file.");
                }

                var taxTable = new TaxTable
                {
                    Year = trimmedYear,
                    FileName = request.File.FileName,
                    FileUrl = $"/Uploads/TaxTables/{uniqueName}",
                    UploadedByUserId = 0,
                    UploadedDate = DateTime.UtcNow
                };

                await _taxTableRepository.AddTaxTableAsync(taxTable);

                if (taxTable.TaxTableId == 0)
                {
                    throw new InvalidOperationException("TaxTableId was not assigned after adding the tax table.");
                }

                foreach (var entry in taxTableEntries)
                {
                    entry.TaxTableId = taxTable.TaxTableId;
                }

                await _taxTableRepository.AddTaxTableEntriesAsync(taxTableEntries);

                return new TaxTableUploadResponseDto
                {
                    TaxTableId = taxTable.TaxTableId,
                    Year = taxTable.Year,
                    FileName = taxTable.FileName,
                    FileUrl = taxTable.FileUrl,
                    UploadedDate = taxTable.UploadedDate,
                    EntryCount = taxTableEntries.Count
                };
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("UploadTaxTable completed for year {Year}. Total time: {ElapsedMs} ms",
                    request.Year ?? "Unknown", stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<List<TaxTableHistoryDto>> GetTaxTableHistoryAsync()
        {
            var taxTables = await _taxTableRepository.GetTaxTableHistoryAsync();
            return taxTables.Select(t => new TaxTableHistoryDto
            {
                TaxTableId = t.TaxTableId,
                Year = t.Year,
                FileName = t.FileName,
                FileUrl = t.FileUrl,
                UploadedDate = t.UploadedDate,
                UploadedByUserId = t.UploadedByUserId ?? 0
            }).ToList();
        }

        private void ValidateUploadRequest(string year, IFormFile file)
        {
            if (string.IsNullOrEmpty(year))
                throw new ArgumentException("Year is required.");

            var years = year.Split('-');
            if (years.Length != 2)
                throw new ArgumentException("Year must be provided in 'YYYY-YYYY' format.");

            if (!int.TryParse(years[0], out int startYear) || !int.TryParse(years[1], out int endYear))
                throw new ArgumentException("Year parts must be valid integers.");

            if (startYear < 1900 || endYear > DateTime.UtcNow.Year + 1 || startYear >= endYear)
                throw new ArgumentException($"Invalid year range. Start year must be >= 1900, end year <= {DateTime.UtcNow.Year + 1}, and start year < end year.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            var allowedExtensions = new[] { ".xls", ".xlsx" };
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                throw new ArgumentException("Only Excel files (.xls or .xlsx) are allowed.");
        }

        private async Task SaveFileAsync(IFormFile file, string filePath)
        {
            var fileStopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                fileStopwatch.Stop();
                _logger.LogInformation("File saved successfully: {FilePath} (Size: {FileSize} bytes, Time: {ElapsedMs} ms)",
                    filePath, file.Length, fileStopwatch.ElapsedMilliseconds);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to save file to {FilePath}", filePath);
                throw;
            }
        }

        private async Task<List<TaxTableEntry>> ParseExcelFileAsync(IFormFile file, string year)
        {
            var excelStopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var taxTableEntries = new List<TaxTableEntry>();
                int rowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                _logger.LogInformation("Processing Excel file with {RowCount} rows", rowCount);

                for (int row = 3; row <= rowCount; row++)
                {
                    var rem1 = worksheet.Cell(row, 1).GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(rem1))
                    {
                        var record = ParseClosedXmlRow(worksheet, row, 1);
                        if (record != null)
                        {
                            taxTableEntries.Add(record);
                            _logger.LogDebug("Parsed row {Row} (col 1): {Remuneration}", row, record.Remuneration);
                        }
                    }

                    var rem2 = worksheet.Cell(row, 8).GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(rem2))
                    {
                        var record = ParseClosedXmlRow(worksheet, row, 8);
                        if (record != null)
                        {
                            taxTableEntries.Add(record);
                            _logger.LogDebug("Parsed row {Row} (col 8): {Remuneration}", row, record.Remuneration);
                        }
                    }
                }

                excelStopwatch.Stop();
                _logger.LogInformation("Excel parsing completed: {EntryCount} entries parsed in {ElapsedMs} ms",
                    taxTableEntries.Count, excelStopwatch.ElapsedMilliseconds);

                return taxTableEntries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Excel file '{FileName}' for year {Year}", file.FileName, year);
                throw;
            }
        }

        private TaxTableEntry? ParseClosedXmlRow(IXLWorksheet worksheet, int row, int startCol)
        {
            _logger.LogDebug("Parsing row {Row}, starting at column {StartCol}", row, startCol);
            try
            {
                var startPart = worksheet.Cell(row, startCol).GetString()?.Trim();
                var dash = worksheet.Cell(row, startCol + 1).GetString()?.Trim();
                var endPart = worksheet.Cell(row, startCol + 2).GetString()?.Trim();

                if (string.IsNullOrEmpty(startPart) || dash != "-" || string.IsNullOrEmpty(endPart))
                {
                    _logger.LogWarning("Invalid data format in row {Row}: startPart={StartPart}, dash={Dash}, endPart={EndPart}",
                        row, startPart, dash, endPart);
                    return null;
                }

                var fullRemunerationText = $"{startPart} - {endPart}";
                var cleanedRemuneration = Regex.Replace(fullRemunerationText, @"R\s*|\s*,\s*", "")
                                             .Replace(" - ", "-")
                                             .Trim();

                if (!Regex.IsMatch(cleanedRemuneration, @"^\d+-\d+$"))
                {
                    _logger.LogWarning("Invalid remuneration format in row {Row}: {CleanedRemuneration}", row, cleanedRemuneration);
                    return null;
                }

                var annualText = worksheet.Cell(row, startCol + 3).GetString()?.Trim();
                if (!decimal.TryParse(Regex.Replace(annualText ?? "", @"[^\d.]", ""), out var annual))
                {
                    _logger.LogWarning("Invalid annual equivalent in row {Row}: {AnnualText}", row, annualText);
                    return null;
                }

                var taxText = worksheet.Cell(row, startCol + 4).GetString()?.Trim() ?? "0";
                if (!decimal.TryParse(Regex.Replace(taxText, @"[^\d.]", ""), out var tax))
                {
                    _logger.LogWarning("Invalid tax value in row {Row}: {TaxText}", row, taxText);
                    return null;
                }

                var entry = new TaxTableEntry
                {
                    Remuneration = cleanedRemuneration,
                    AnnualEquivalent = annual,
                    TaxUnder65 = tax
                };
                _logger.LogDebug("Successfully parsed row {Row}: Remuneration={Remuneration}, Annual={Annual}, Tax={Tax}",
                    row, entry.Remuneration, entry.AnnualEquivalent, entry.TaxUnder65);
                return entry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse row {Row}, column {StartCol}", row, startCol);
                return null;
            }
        }
    }
}