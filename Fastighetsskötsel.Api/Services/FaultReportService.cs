using Fastighetsskötsel.Api.Data.DTO;
using Fastighetsskötsel.Api.Data.Entities;
using Fastighetsskötsel.Api.Data.Repositories.Interfaces;
using Fastighetsskötsel.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fastighetsskötsel.Api.Services;

public class FaultReportService : IFaultReportService
{
    #region Dependencies
    private readonly IFaultReportRepository _repository;
    private readonly ISMSNotifyer _smsNotifyer;
    private readonly ILogger<FaultReportService> _logger;
    #endregion

    #region Constructor
    public FaultReportService(IFaultReportRepository repository, ISMSNotifyer smsNotifyer, ILogger<FaultReportService> logger)
    {
        _repository = repository;
        _smsNotifyer = smsNotifyer;
        _logger = logger;
    }
    #endregion

    public async Task<FaultReportDto> CreateAsync(FaultReportCreateDto dto, string createdBy)
    {
        var faultReport = new FaultReport
        {
            Description = dto.Description,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            Status = FaultReportStatus.Reported
        };

        var createdReport = await _repository.CreateAsync(faultReport);

        try
        {
            await _smsNotifyer.NotifyAsync(
                $"Ny felanmälan skapad: {createdReport.Description}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Kunde inte skicka SMS-notifiering för felanmälan {FaultReportId}.",
                createdReport.Id);
        }

        return MapToDto(createdReport);
    }

    public async Task<IEnumerable<FaultReportDto>> GetOwnAsync(string createdBy)
    {
        var reports = await _repository.GetByCreatedByAsync(createdBy);

        return reports.Select(MapToDto);
    }

    public async Task<IEnumerable<FaultReportDto>> GetAllAsync()
    {
        var reports = await _repository.GetAllAsync();

        return reports.Select(MapToDto);
    }

    public async Task<FaultReportDto?> GetByIdAsync(int id)
    {
        var faultReport = await _repository.GetByIdAsync(id);

        if (faultReport is null)
        {
            return null;
        }

        return MapToDto(faultReport);
    }

    public async Task<FaultReportDto?> UpdateAsync(int id, FaultReportUpdateDto dto)
    {
        var faultReport = await _repository.GetByIdAsync(id);

        if (faultReport is null)
        {
            return null;
        }

        faultReport.Status = dto.Status;

        var updatedReport = await _repository.UpdateAsync(faultReport);

        return MapToDto(updatedReport);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var faultReport = await _repository.GetByIdAsync(id);

        if (faultReport is null)
        {
            return false;
        }

        await _repository.DeleteAsync(faultReport);

        return true;
    }

    private static FaultReportDto MapToDto(FaultReport faultReport)
    {
        return new FaultReportDto
        {
            Id = faultReport.Id,
            Description = faultReport.Description,
            CreatedBy = faultReport.CreatedBy,
            CreatedAt = faultReport.CreatedAt,
            Status = faultReport.Status
        };
    }
}