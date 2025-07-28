using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SingularExpress.Models;
using SingularExpress.Models.Models;
using Microsoft.Extensions.Logging;

namespace SingularExpress.Api.Repositories
{
    public class TaxTableRepository : ITaxTableRepository
    {
        private readonly ModelDbContext _context;
        private readonly ILogger<TaxTableRepository> _logger;

        public TaxTableRepository(ModelDbContext context, ILogger<TaxTableRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TaxTable>> GetAllTaxTablesAsync()
        {
            return await _context.TaxTables.ToListAsync();
        }

        public async Task<TaxTable?> GetTaxTableByIdAsync(int id)
        {
            return await _context.TaxTables.FindAsync(id);
        }

        public async Task AddTaxTableAsync(TaxTable taxTable)
        {
            await DatabaseOperationWithLogging(async () =>
            {
                _context.TaxTables.Add(taxTable);
                await _context.SaveChangesAsync();
            }, "add tax table for year {Year}", taxTable.Year ?? "Unknown");
        }

        public async Task UpdateTaxTableAsync(TaxTable taxTable)
        {
            await DatabaseOperationWithLogging(async () =>
            {
                _context.Entry(taxTable).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }, "update tax table with ID {Id}", taxTable.TaxTableId);
        }

        public async Task DeleteTaxTableAsync(int id)
        {
            await DatabaseOperationWithLogging(async () =>
            {
                var taxTable = await _context.TaxTables.FindAsync(id);
                if (taxTable != null)
                {
                    _context.TaxTables.Remove(taxTable);
                    await _context.SaveChangesAsync();
                }
            }, "delete tax table with ID {Id}", id);
        }

        public async Task<bool> TaxTableExistsAsync(int id)
        {
            var exists = await _context.TaxTables.AnyAsync(e => e.TaxTableId == id);
            _logger.LogDebug("Checked existence of tax table with ID {Id}: {Exists}", id, exists);
            return exists;
        }

        public async Task AddTaxTableEntriesAsync(List<TaxTableEntry> entries)
        {
            const int batchSize = 100;
            var dbStopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                for (int i = 0; i < entries.Count; i += batchSize)
                {
                    var batch = entries.Skip(i).Take(batchSize).ToList();
                    var firstEntry = batch.FirstOrDefault();
                    await DatabaseOperationWithLogging(async () =>
                    {
                        await _context.TaxTableEntries.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                    }, "save batch of {Count} entries for TaxTableId {Id}", batch.Count, firstEntry?.TaxTableId ?? 0);
                    _logger.LogInformation("Saved batch of {Count} entries for TaxTableId {Id}", batch.Count, firstEntry?.TaxTableId ?? 0);
                }
            }
            finally
            {
                dbStopwatch.Stop();
                _logger.LogInformation("Database operations completed: {EntryCount} entries saved in {ElapsedMs} ms",
                    entries.Count, dbStopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<List<TaxTable>> GetTaxTableHistoryAsync()
        {
            return await _context.TaxTables
                .OrderByDescending(t => t.UploadedDate)
                .ToListAsync();
        }

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