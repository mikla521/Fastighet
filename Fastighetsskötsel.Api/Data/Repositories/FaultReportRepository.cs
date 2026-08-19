using Fastighetsskötsel.Api.Data;
using Fastighetsskötsel.Api.Data.Entities;
using Fastighetsskötsel.Api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fastighetsskötsel.Api.Data.Repositories;

public class FaultReportRepository : IFaultReportRepository
{
    private readonly ApplicationDbContext _context;

    public FaultReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FaultReport> CreateAsync(FaultReport faultReport)
    {
        _context.FaultReports.Add(faultReport);
        await _context.SaveChangesAsync();

        return faultReport;
    }

    public async Task<IEnumerable<FaultReport>> GetByCreatedByAsync(string createdBy)
    {
        return await _context.FaultReports
            .Where(x => x.CreatedBy == createdBy)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FaultReport>> GetAllAsync()
    {
        return await _context.FaultReports
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    public async Task<FaultReport?> GetByIdAsync(int id)
    {
        return await _context.FaultReports
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<FaultReport> UpdateAsync(FaultReport faultReport)
    {
        _context.FaultReports.Update(faultReport);
        await _context.SaveChangesAsync();

        return faultReport;
    }

    public async Task<bool> DeleteAsync(FaultReport faultReport)
    {
        _context.FaultReports.Remove(faultReport);
        await _context.SaveChangesAsync();

        return true;
    }
}