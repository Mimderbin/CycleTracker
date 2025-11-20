using API.Data;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PKController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PkService _pkService;

    public PKController(AppDbContext context, PkService pkService)
    {
        _context = context;
        _pkService = pkService;
    }

    [HttpGet("cycle/{cycleId}")]
    public async Task<IActionResult> GetForCycle(int cycleId)
    {
        var cycle = await _context.Cycles
            .Include(c => c.DoseEvents)
            .ThenInclude(d => d.Compound)
            .FirstOrDefaultAsync(c => c.Id == cycleId);

        if (cycle == null)
            return NotFound();

        var result = _pkService.CalculateLevels(cycle);
        return Ok(result);
    }
}
