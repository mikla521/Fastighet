using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fastighetsskötsel.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthTestController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Resident")]
    public IActionResult Get()
    {
        return Ok("Autentisering fungerar.");
    }
}
