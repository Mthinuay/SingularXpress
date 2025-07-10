using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SingularExpress.Models;
using SingularExpress.Models.Models;
using ClosedXML.Excel;

namespace SingularExpress.Api.Controllers
{
    [ApiController]
    [Route("api/tax-tables")]
    public class TaxTableController : ControllerBase
    {
        private readonly ModelDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TaxTableController> _logger;

        public TaxTableController(ModelDbContext context, IWebHostEnvironment env, ILogger<TaxTableController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("TaxTableController initialized");
        }

        public class TaxTableUploadRequest
        {
            [FromForm]
            public string Year { get; set; } = string.Empty;

            [FromForm]
            public IFormFile File { get; set; } = null!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxTable>>> GetTaxTables()
        {
            _logger.LogInformation("Entering GetTaxTables");
            try
            {
                var taxTables = await _context.TaxTables.ToListAsync();
                _logger.LogInformation("Retrieved {Count} tax tables", taxTables.Count);
                return Ok(taxTables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax tables");
                return StatusCode(500, new { message = "An error occurred while retrieving tax tables." });
            }
            finally
            {
                _logger.LogInformation("Exiting GetTaxTables");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxTable>> GetTaxTable(int id)
        {
            _logger.LogInformation("Entering GetTaxTable with ID {Id}", id);
            try
            {
                var taxTable = await _context.TaxTables.FindAsync(id);

                if (taxTable == null)
                {
                    _logger.LogWarning("Tax table with ID {Id} not found", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }

                _logger.LogInformation("Retrieved tax table with ID {Id}", id);
                return Ok(taxTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the tax table." });
            }
            finally
            {
                _logger.LogInformation("Exiting GetTaxTable with ID {Id}", id);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaxTable>> PostTaxTable(TaxTable taxTable)
        {
            _logger.LogInformation("Entering PostTaxTable for year {Year}", taxTable.Year);
            try
            {
                if (string.IsNullOrEmpty(taxTable.Year) || !Regex.IsMatch(taxTable.Year, @"^\d{4}-\d{4}$"))
                {
                    _logger.LogWarning("Invalid year format: {Year}", taxTable.Year);
                    return BadRequest(new { message = "Invalid year format. Use 'YYYY-YYYY'." });
                }

                _context.TaxTables.Add(taxTable);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tax table created with ID {Id} for year {Year}", taxTable.TaxTableId, taxTable.Year);
                return CreatedAtAction(nameof(GetTaxTable), new { id = taxTable.TaxTableId }, taxTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tax table for year {Year}", taxTable.Year);
                return StatusCode(500, new { message = "An error occurred while creating the tax table." });
            }
            finally
            {
                _logger.LogInformation("Exiting PostTaxTable for year {Year}", taxTable.Year);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxTable(int id, TaxTable taxTable)
        {
            _logger.LogInformation("Entering PutTaxTable with ID {Id}", id);
            try
            {
                if (id != taxTable.TaxTableId)
                {
                    _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", id, taxTable.TaxTableId);
                    return BadRequest(new { message = "Tax table ID mismatch." });
                }

                if (string.IsNullOrEmpty(taxTable.Year) || !Regex.IsMatch(taxTable.Year, @"^\d{4}-\d{4}$"))
                {
                    _logger.LogWarning("Invalid year format: {Year}", taxTable.Year);
                    return BadRequest(new { message = "Invalid year format. Use 'YYYY-YYYY'." });
                }

                _context.Entry(taxTable).State = EntityState.Modified;
                await DatabaseOperationWithLogging(async () => await _context.SaveChangesAsync(), "update tax table with ID {Id}", id);
                _logger.LogInformation("Tax table with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TaxTableExists(id))
                {
                    _logger.LogWarning("Tax table with ID {Id} not found during update", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }
                _logger.LogError(ex, "Concurrency error updating tax table with ID {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the tax table." });
            }
            finally
            {
                _logger.LogInformation("Exiting PutTaxTable with ID {Id}", id);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxTable(int id)
        {
            _logger.LogInformation("Entering DeleteTaxTable with ID {Id}", id);
            try
            {
                var taxTable = await _context.TaxTables.FindAsync(id);
                if (taxTable == null)
                {
                    _logger.LogWarning("Tax table with ID {Id} not found", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }

                _context.TaxTables.Remove(taxTable);
                await DatabaseOperationWithLogging(async () => await _context.SaveChangesAsync(), "delete tax table with ID {Id}", id);
                _logger.LogInformation("Tax table with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the tax table." });
            }
            finally
            {
                _logger.LogInformation("Exiting DeleteTaxTable with ID {Id}", id);
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
        public async Task<IActionResult> UploadTaxTable([FromForm] TaxTableUploadRequest request)
        {
            string? trimmedYear = null;
            _logger.LogInformation("Entering UploadTaxTable with Year: '{Year}' and File: '{FileName}' (Size: {FileSize} bytes)", 
                request.Year, request.File?.FileName, request.File?.Length);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                trimmedYear = request.Year?.Trim().Trim('"').Trim('\'');
                _logger.LogInformation("Processed year: {TrimmedYear}", trimmedYear);

                if (string.IsNullOrEmpty(trimmedYear))
                {
                    _logger.LogWarning("Upload failed: Year is empty or null");
                    return BadRequest(new { message = "Year is required." });
                }

                var years = trimmedYear.Split('-');
                if (years.Length != 2)
                {
                    _logger.LogWarning("Upload failed: Invalid year format '{Year}'", trimmedYear);
                    return BadRequest(new { message = "Year must be provided in 'YYYY-YYYY' format." });
                }

                if (!int.TryParse(years[0], out int startYear) || !int.TryParse(years[1], out int endYear))
                {
                    _logger.LogWarning("Upload failed: Year parts are not valid integers '{Year}'", trimmedYear);
                    return BadRequest(new { message = "Year parts must be valid integers." });
                }

                if (startYear < 1900 || endYear > DateTime.UtcNow.Year + 1 || startYear >= endYear)
                {
                    _logger.LogWarning("Upload failed: Invalid year range '{Year}' (Start: {StartYear}, End: {EndYear})", 
                        trimmedYear, startYear, endYear);
                    return BadRequest(new
                    {
                        message = $"Invalid year range. Start year must be >= 1900, end year <= {DateTime.UtcNow.Year + 1}, and start year < end year."
                    });
                }

                if (request.File == null || request.File.Length == 0)
                {
                    _logger.LogWarning("Upload failed: No file provided or file is empty");
                    return BadRequest(new { message = "No file provided." });
                }

                var allowedExtensions = new[] { ".xls", ".xlsx" };
                var extension = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("Upload failed: Invalid file extension '{Extension}' for file '{FileName}'", 
                        extension, request.File.FileName);
                    return BadRequest(new { message = "Only Excel files (.xls or .xlsx) are allowed." });
                }

                var folder = Path.Combine(_env.ContentRootPath, "Uploads", "TaxTables");
                Directory.CreateDirectory(folder);

                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folder, uniqueName);

                try
                {
                    var fileStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await request.File.CopyToAsync(stream);
                    fileStopwatch.Stop();
                    _logger.LogInformation("File saved successfully: {FilePath} (Size: {FileSize} bytes, Time: {ElapsedMs} ms)", 
                        filePath, request.File.Length, fileStopwatch.ElapsedMilliseconds);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Failed to save file to {FilePath}", filePath);
                    return StatusCode(500, new { message = "Failed to save the uploaded file." });
                }

                try
                {
                    var excelStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    await using var stream = request.File.OpenReadStream();
                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheet(1);

                    var taxTableEntries = new List<TaxTableEntry>();
                    int rowCount = worksheet.LastRowUsed().RowNumber();
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
                            else
                            {
                                _logger.LogWarning("Failed to parse row {Row} (col 1)", row);
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
                            else
                            {
                                _logger.LogWarning("Failed to parse row {Row} (col 8)", row);
                            }
                        }
                    }

                    excelStopwatch.Stop();
                    _logger.LogInformation("Excel parsing completed: {EntryCount} entries parsed in {ElapsedMs} ms", 
                        taxTableEntries.Count, excelStopwatch.ElapsedMilliseconds);

                    if (!taxTableEntries.Any())
                    {
                        _logger.LogWarning("No valid entries parsed from Excel file '{FileName}'", request.File.FileName);
                        return BadRequest(new { message = "No valid tax table entries found in the Excel file." });
                    }

                    var taxTable = new TaxTable
                    {
                        Year = trimmedYear,
                        FileName = request.File.FileName,
                        FileUrl = $"/Uploads/TaxTables/{uniqueName}",
                        UploadedByUserId = 0,
                        UploadedDate = DateTime.UtcNow
                    };

                    _context.TaxTables.Add(taxTable);
                    await DatabaseOperationWithLogging(async () => await _context.SaveChangesAsync(), 
                        "save tax table for year {Year}", trimmedYear);
                    _logger.LogInformation("Tax table created with ID {Id} for year {Year}", taxTable.TaxTableId, trimmedYear);

                    foreach (var entry in taxTableEntries)
                    {
                        entry.TaxTableId = taxTable.TaxTableId;
                    }

                    const int batchSize = 100;
                    var dbStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    for (int i = 0; i < taxTableEntries.Count; i += batchSize)
                    {
                        var batch = taxTableEntries.Skip(i).Take(batchSize).ToList();
                        await _context.TaxTableEntries.AddRangeAsync(batch);
                        await DatabaseOperationWithLogging(async () => await _context.SaveChangesAsync(), 
                            "save batch of {Count} entries for TaxTableId {Id}", batch.Count, taxTable.TaxTableId);
                        _logger.LogInformation("Saved batch of {Count} entries for TaxTableId {Id}", batch.Count, taxTable.TaxTableId);
                    }
                    dbStopwatch.Stop();
                    _logger.LogInformation("Database operations completed: {EntryCount} entries saved in {ElapsedMs} ms", 
                        taxTableEntries.Count, dbStopwatch.ElapsedMilliseconds);

                    _logger.LogInformation("Uploaded {Count} tax table entries for TaxTableId {Id}", taxTableEntries.Count, taxTable.TaxTableId);
                    return Ok(new
                    {
                        taxTable.TaxTableId,
                        taxTable.Year,
                        taxTable.FileName,
                        taxTable.FileUrl,
                        taxTable.UploadedDate,
                        EntryCount = taxTableEntries.Count,
                        message = $"Tax table uploaded successfully with {taxTableEntries.Count} entries."
                    });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while saving tax table or entries for year {Year}. InnerException: {InnerMessage}", 
                        trimmedYear, ex.InnerException?.Message);
                    return StatusCode(500, new { message = "Failed to save data to the database." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Excel file '{FileName}' for year {Year}", request.File?.FileName, trimmedYear);
                    return StatusCode(500, new { message = "Failed to process the Excel file." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during upload for year {Year}", trimmedYear);
                return StatusCode(500, new { message = "An unexpected error occurred during upload." });
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Exiting UploadTaxTable for year {Year}. Total time: {ElapsedMs} ms", 
                    trimmedYear, stopwatch.ElapsedMilliseconds);
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            _logger.LogInformation("Entering GetHistory");
            try
            {
                var history = await _context.TaxTables
                    .OrderByDescending(t => t.UploadedDate)
                    .Select(t => new
                    {
                        t.TaxTableId,
                        t.Year,
                        t.FileName,
                        t.FileUrl,
                        t.UploadedDate,
                        t.UploadedByUserId
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} history records", history.Count);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax table history");
                return StatusCode(500, new { message = "An error occurred while retrieving tax table history." });
            }
            finally
            {
                _logger.LogInformation("Exiting GetHistory");
            }
        }

        private bool TaxTableExists(int id)
        {
            var exists = _context.TaxTables.Any(e => e.TaxTableId == id);
            _logger.LogDebug("Checked existence of tax table with ID {Id}: {Exists}", id, exists);
            return exists;
        }

#pragma warning disable CS8603
        private TaxTableEntry? ParseClosedXmlRow(IXLWorksheet worksheet, int row, int startCol)
        {
            _logger.LogDebug("Parsing row {Row}, starting at column {StartCol}", row, startCol);
            try
            {
                var startPart = worksheet.Cell(row, startCol).GetString()?.Trim();
                var dash = worksheet.Cell(row, startCol + 1).GetString()?.Trim();
                var endPart = worksheet.Cell(row, startCol + 2).GetString()?.Trim();

                _logger.LogDebug("Row {Row} data: startPart={StartPart}, dash={Dash}, endPart={EndPart}", 
                    row, startPart, dash, endPart);

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

                _logger.LogDebug("Row {Row}: Cleaned remuneration: {CleanedRemuneration}", row, cleanedRemuneration);

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
#pragma warning restore CS8603

        private async Task DatabaseOperationWithLogging(Func<Task> operation, string operationDescription, params object[] args)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await operation();
                stopwatch.Stop();
                _logger.LogInformation("Database operation succeeded: " + operationDescription + " (Time: {ElapsedMs} ms)", 
                    args.Concat(new object[] { stopwatch.ElapsedMilliseconds }).ToArray());
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Database operation failed: " + operationDescription + " (Time: {ElapsedMs} ms)", 
                    args.Concat(new object[] { stopwatch.ElapsedMilliseconds }).ToArray());
                throw;
            }
        }
    }
}