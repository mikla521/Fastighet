using Fastighetsskötsel.Api.Data.DTO;

namespace Fastighetsskötsel.Api.Services.Interfaces;

public interface IFaultReportService
{
    Task<FaultReportDto> CreateAsync(FaultReportCreateDto dto, string createdBy);
    Task<IEnumerable<FaultReportDto>> GetOwnAsync(string createdBy);
    Task<IEnumerable<FaultReportDto>> GetAllAsync();
    Task<FaultReportDto?> GetByIdAsync(int id);
    Task<FaultReportDto?> UpdateAsync(int id, FaultReportUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}