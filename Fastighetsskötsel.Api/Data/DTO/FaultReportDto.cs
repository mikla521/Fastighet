using Fastighetsskötsel.Api.Data.Entities;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

namespace Fastighetsskötsel.Api.Data.DTO;

public class FaultReportDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public FaultReportStatus Status { get; set; }
}
