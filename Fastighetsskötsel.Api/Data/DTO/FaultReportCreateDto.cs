using System.ComponentModel.DataAnnotations;

namespace Fastighetsskötsel.Api.Data.DTO;

public class FaultReportCreateDto
{
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
}
