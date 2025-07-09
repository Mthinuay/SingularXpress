using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using SingularExpress.Models;
using SingularExpress.Models.Models;

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
            _context = context;
            _env = env;
            _logger = logger;
        }

        // DTO for upload
        public class TaxTableUploadRequest
        {
            [FromForm]
            public string Year { get; set; } = string.Empty;

            [FromForm]
            public IFormFile File { get; set; } = null!;
        }

        // GET: api/tax-tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxTable>>> GetTaxTables()
        {
            try
            {
                _logger.LogInformation("Retrieving all tax tables");
                var taxTables = await _context.TaxTables.ToListAsync();
                return Ok(taxTables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax tables");
                return StatusCode(500, new { message = "An error occurred while retrieving tax tables." });
            }
        }

        // GET: api/tax-tables/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaxTable>> GetTaxTable(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving tax table with ID {Id}", id);
                var taxTable = await _context.TaxTables.FindAsync(id);

                if (taxTable == null)
                {
                    _logger.LogWarning("Tax table with ID {Id} not found", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }

                return Ok(taxTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the tax table." });
            }
        }

        // POST: api/tax-tables
        [HttpPost]
        public async Task<ActionResult<TaxTable>> PostTaxTable(TaxTable taxTable)
        {
            try
            {
                _logger.LogInformation("Creating new tax table for year {Year}", taxTable.Year);

                if (!int.TryParse(taxTable.Year, out var parsedYear) || parsedYear < 1900 || parsedYear > DateTime.Now.Year + 1)
                {
                    return BadRequest(new { message = "Invalid year provided. Year must be between 1900 and next year." });
                }

                _context.TaxTables.Add(taxTable);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tax table created with ID {Id}", taxTable.TaxTableId);
                return CreatedAtAction(nameof(GetTaxTable), new { id = taxTable.TaxTableId }, taxTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tax table for year {Year}", taxTable.Year);
                return StatusCode(500, new { message = "An error occurred while creating the tax table." });
            }
        }

        // PUT: api/tax-tables/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxTable(int id, TaxTable taxTable)
        {
            try
            {
                _logger.LogInformation("Updating tax table with ID {Id}", id);

                if (id != taxTable.TaxTableId)
                {
                    _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", id, taxTable.TaxTableId);
                    return BadRequest(new { message = "Tax table ID mismatch." });
                }

                if (!int.TryParse(taxTable.Year, out var parsedYear) || parsedYear < 1900 || parsedYear > DateTime.Now.Year + 1)
                {
                    return BadRequest(new { message = "Invalid year provided. Year must be between 1900 and next year." });
                }

                _context.Entry(taxTable).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Tax table with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxTableExists(id))
                {
                    _logger.LogWarning("Tax table with ID {Id} not found during update", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }
                _logger.LogError("Concurrency error updating tax table with ID {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the tax table." });
            }
        }

        // DELETE: api/tax-tables/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxTable(int id)
        {
            try
            {
                _logger.LogInformation("Deleting tax table with ID {Id}", id);
                var taxTable = await _context.TaxTables.FindAsync(id);
                if (taxTable == null)
                {
                    _logger.LogWarning("Tax table with ID {Id} not found", id);
                    return NotFound(new { message = $"Tax table with ID {id} not found." });
                }

                _context.TaxTables.Remove(taxTable);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tax table with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tax table with ID {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the tax table." });
            }
        }

      [HttpPost("upload")]
[Consumes("multipart/form-data")]
[RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)] // 100 MB limit
public async Task<IActionResult> UploadTaxTable([FromForm] TaxTableUploadRequest request)
{
    try
    {
        // Defensive trimming and removing quotes (single or double) if present
        var trimmedYear = request.Year?.Trim().Trim('"').Trim('\'');
        _logger.LogInformation("Received upload request with Year: '{Year}'", trimmedYear);

        // Validate year format: exactly two years separated by '-'
        var years = trimmedYear?.Split('-');
        if (years == null || years.Length != 2)
        {
            return BadRequest(new { message = "Year must be provided in 'YYYY-YYYY' format." });
        }

        // Parse start and end years
        bool isStartValid = int.TryParse(years[0], out int startYear);
        bool isEndValid = int.TryParse(years[1], out int endYear);

        if (!isStartValid || !isEndValid)
        {
            return BadRequest(new { message = "Year parts must be valid integers." });
        }

        // Validate reasonable year range
        if (startYear < 1900 || endYear > DateTime.Now.Year + 1 || startYear >= endYear)
        {
            return BadRequest(new
            {
                message = $"Invalid year range. Start year must be >= 1900, end year <= {DateTime.Now.Year + 1}, and start year < end year."
            });
        }

        // Validate file presence
        if (request.File == null || request.File.Length == 0)
        {
            _logger.LogWarning("Upload failed: No file provided");
            return BadRequest(new { message = "No file provided." });
        }

        // Validate allowed file extensions
        var allowedExtensions = new[] { ".xls", ".xlsx" };
        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Upload failed: Invalid file extension '{Extension}'", extension);
            return BadRequest(new { message = "Only Excel files (.xls or .xlsx) are allowed." });
        }

        // Prepare save folder
        var folder = Path.Combine(_env.ContentRootPath, "Uploads", "TaxTables");
        if (!Directory.Exists(folder))
        {
            _logger.LogInformation("Creating upload directory: {Folder}", folder);
            Directory.CreateDirectory(folder);
        }

        // Save file with unique name
        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folder, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        _logger.LogInformation("File saved successfully: {FilePath}", filePath);

        // Create new TaxTable record
        var taxTable = new TaxTable
        {
            Year = trimmedYear,
            FileName = request.File.FileName,
            FileUrl = $"/Uploads/TaxTables/{uniqueName}",
            UploadedByUserId = 0, // replace with actual user id if available
            UploadedDate = DateTime.UtcNow
        };

        _context.TaxTables.Add(taxTable);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tax table record created successfully: ID {TaxTableId}", taxTable.TaxTableId);

        // Return success response
        return Ok(new
        {
            taxTable.TaxTableId,
            taxTable.Year,
            taxTable.FileName,
            taxTable.FileUrl,
            taxTable.UploadedDate,
            message = "Tax table uploaded successfully."
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while uploading tax table for year {Year}", request.Year);
        return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
    }
}

        // GET: api/tax-tables/history
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                _logger.LogInformation("Retrieving tax table history");
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

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tax table history");
                return StatusCode(500, new { message = "An error occurred while retrieving tax table history." });
            }
        }

        private bool TaxTableExists(int id)
        {
            return _context.TaxTables.Any(e => e.TaxTableId == id);
        }
    }
}
