using Fastighetsskötsel.Api.Data.DTO;
using Fastighetsskötsel.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fastighetsskötsel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaultReportsController : ControllerBase
{
    #region Dependencies
    private readonly IFaultReportService _service; 
    #endregion

    #region Constructor
    public FaultReportsController(IFaultReportService service)
    {
        _service = service;
    } 
    #endregion

    #region POST
    [HttpPost]
    [Authorize(Roles = "Resident")]
    public async Task<ActionResult<FaultReportDto>> Create(FaultReportCreateDto dto)
    {
        var objectId = User.FindFirst(
            "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (objectId is null)
        {
            return Unauthorized();
        }

        var faultReport = await _service.CreateAsync(dto, objectId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = faultReport.Id },
            faultReport);
    } 
    #endregion

    #region GET
    [HttpGet("mine")]
    [Authorize(Roles = "Resident")]
    public async Task<ActionResult<IEnumerable<FaultReportDto>>> GetMine()
    {
        var objectId = User.FindFirst(
            "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (objectId is null)
        {
            return Unauthorized();
        }

        var faultReports = await _service.GetOwnAsync(objectId);

        return Ok(faultReports);
    }

    [HttpGet]
    [Authorize(Roles = "PropertyManager")]
    public async Task<ActionResult<IEnumerable<FaultReportDto>>> GetAll()
    {
        var faultReports = await _service.GetAllAsync();

        return Ok(faultReports);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<ActionResult<FaultReportDto>> GetById(int id)
    {
        var faultReport = await _service.GetByIdAsync(id);

        if (faultReport is null)
        {
            return NotFound();
        }

        return Ok(faultReport);
    }
    #endregion

    #region PATCH
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<ActionResult<FaultReportDto>> Update(
int id,
FaultReportUpdateDto dto)
    {
        var faultReport = await _service.UpdateAsync(id, dto);

        if (faultReport is null)
        {
            return NotFound();
        }

        return Ok(faultReport);
    }
    #endregion

    #region DELETE
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "PropertyManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    } 
    #endregion
}
