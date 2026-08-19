namespace Fastighetsskötsel.Api.Data.Entities;

public class FaultReport
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public FaultReportStatus Status { get; set; }
}
