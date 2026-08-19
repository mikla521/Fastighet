using Fastighetsskötsel.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fastighetsskötsel.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<FaultReport> FaultReports => Set<FaultReport>();
}