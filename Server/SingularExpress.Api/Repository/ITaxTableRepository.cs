using System.Collections.Generic;
using System.Threading.Tasks;
using SingularExpress.Models;
using SingularExpress.Models.Models;

namespace SingularExpress.Api.Repositories
{
    public interface ITaxTableRepository
    {
        Task<List<TaxTable>> GetAllTaxTablesAsync();
        Task<TaxTable?> GetTaxTableByIdAsync(int id);
        Task AddTaxTableAsync(TaxTable taxTable);
        Task UpdateTaxTableAsync(TaxTable taxTable);
        Task DeleteTaxTableAsync(int id);
        Task<bool> TaxTableExistsAsync(int id);
        Task AddTaxTableEntriesAsync(List<TaxTableEntry> entries);
        Task<List<TaxTable>> GetTaxTableHistoryAsync();
    }
}