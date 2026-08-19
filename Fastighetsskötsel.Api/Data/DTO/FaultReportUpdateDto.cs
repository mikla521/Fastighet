using Fastighetsskötsel.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Fastighetsskötsel.Api.Data.DTO;

public class FaultReportUpdateDto
{
    [EnumDataType(typeof(FaultReportStatus))]
    public FaultReportStatus Status { get; set; }
}
