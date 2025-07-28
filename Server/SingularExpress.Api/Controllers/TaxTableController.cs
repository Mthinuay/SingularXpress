using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SingularExpress.Api.Services;
using SingularExpress.Api.Dtos;

namespace SingularExpress.Api.Controllers
{
    [ApiController]
    [Route("api/tax-tables")]
    public class TaxTableController : ControllerBase
    {
        private readonly ITaxTableService _taxTableService;
        private readonly ILogger<TaxTableController> _logger;

        public TaxTableController(ITaxTableService taxTableService, ILogger<TaxTableController> logger)
        {
            _taxTableService = taxTableService ?? throw new ArgumentNullException(nameof(taxTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("TaxTableController initialized");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxTableDto>>> GetTaxTables()
        {
            _logger.LogInformation("Entering GetTaxTables");
            try
            {
                var taxTables = await _taxTableService.GetAllTaxTablesAsync();
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
        public async Task<ActionResult<TaxTableDto>> GetTaxTable(int id)
        {
            _logger.LogInformation("Entering GetTaxTable with ID {Id}", id);
            try
            {
                var taxTable = await _taxTableService.GetTaxTableByIdAsync(id);
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
        public async Task<ActionResult<TaxTableDto>> PostTaxTable(TaxTableDto taxTable)
        {
            _logger.LogInformation("Entering PostTaxTable for year {Year}", taxTable.Year);
            try
            {
                var createdTaxTable = await _taxTableService.CreateTaxTableAsync(taxTable);
                _logger.LogInformation("Tax table created with ID {Id} for year {Year}", createdTaxTable.TaxTableId, createdTaxTable.Year);
                return CreatedAtAction(nameof(GetTaxTable), new { id = createdTaxTable.TaxTableId }, createdTaxTable);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid input: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
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
        public async Task<IActionResult> PutTaxTable(int id, TaxTableDto taxTable)
        {
            _logger.LogInformation("Entering PutTaxTable with ID {Id}", id);
            try
            {
                if (id != taxTable.TaxTableId)
                {
                    _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", id, taxTable.TaxTableId);
                    return BadRequest(new { message = "Tax table ID mismatch." });
                }

                await _taxTableService.UpdateTaxTableAsync(taxTable);
                _logger.LogInformation("Tax table with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid input: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tax table with ID {Id} not found during update", id);
                return NotFound(new { message = ex.Message });
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
                await _taxTableService.DeleteTaxTableAsync(id);
                _logger.LogInformation("Tax table with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tax table with ID {Id} not found", id);
                return NotFound(new { message = ex.Message });
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
        public async Task<IActionResult> UploadTaxTable([FromForm] TaxTableUploadRequestDto request)
        {
            _logger.LogInformation("Entering UploadTaxTable with Year: '{Year}' and File: '{FileName}' (Size: {FileSize} bytes)", 
                request.Year, request.File?.FileName, request.File?.Length);
            try
            {
                var result = await _taxTableService.UploadTaxTableAsync(request);
                _logger.LogInformation("Uploaded {Count} tax table entries for TaxTableId {Id}", result.EntryCount, result.TaxTableId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Upload failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during upload for year {Year}", request.Year);
                return StatusCode(500, new { message = "An unexpected error occurred during upload." });
            }
            finally
            {
                _logger.LogInformation("Exiting UploadTaxTable for year {Year}", request.Year);
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            _logger.LogInformation("Entering GetHistory");
            try
            {
                var history = await _taxTableService.GetTaxTableHistoryAsync();
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
    }
}