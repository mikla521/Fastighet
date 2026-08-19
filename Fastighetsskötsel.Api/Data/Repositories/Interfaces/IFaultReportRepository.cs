using Fastighetsskötsel.Api.Data.Entities;

namespace Fastighetsskötsel.Api.Data.Repositories.Interfaces;

public interface IFaultReportRepository
{
    Task<FaultReport> CreateAsync(FaultReport faultReport);
    Task<IEnumerable<FaultReport>> GetByCreatedByAsync(string createdBy);
    Task<IEnumerable<FaultReport>> GetAllAsync();
    Task<FaultReport?> GetByIdAsync(int id);
    Task<FaultReport> UpdateAsync(FaultReport faultReport);
    Task<bool> DeleteAsync(FaultReport faultReport);
}