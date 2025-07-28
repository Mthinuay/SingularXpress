using System.Collections.Generic;
using System.Threading.Tasks;
using SingularExpress.Api.Dtos;

namespace SingularExpress.Api.Services
{
    public interface ITaxTableService
    {
        Task<List<TaxTableDto>> GetAllTaxTablesAsync();
        Task<TaxTableDto?> GetTaxTableByIdAsync(int id);
        Task<TaxTableDto> CreateTaxTableAsync(TaxTableDto taxTable);
        Task UpdateTaxTableAsync(TaxTableDto taxTable);
        Task DeleteTaxTableAsync(int id);
        Task<TaxTableUploadResponseDto> UploadTaxTableAsync(TaxTableUploadRequestDto request);
        Task<List<TaxTableHistoryDto>> GetTaxTableHistoryAsync();
    }
}