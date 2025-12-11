using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PkController : ControllerBase
{
    private readonly IPkService _pkService;

    public PkController(IPkService pkService)
    {
        _pkService = pkService;
    }

    // GET api/pk/cycle/5
    [HttpGet("cycle/{cycleId:int}")]
    public async Task<IActionResult> GetForCycle(int cycleId, CancellationToken ct)
    {
        var levels = await _pkService.GetLevelsForCycleAsync(cycleId, ct);
        return Ok(levels);
    }
}
