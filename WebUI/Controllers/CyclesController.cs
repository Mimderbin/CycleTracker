using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers;

public class CyclesController : Controller
{
    private readonly HttpClient _client;

    public CyclesController(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _client.GetFromJsonAsync<ODataListResponse<CycleDto>>("/odata/Cycles");
        var cycles = response?.Value ?? new List<CycleDto>();
        return View(cycles);
    }
    public async Task<IActionResult> Details(int id)
    {
        // 1) Get cycle
        var cycle = await _client.GetFromJsonAsync<CycleDto>($"/odata/Cycles({id})");

        if (cycle == null)
            return NotFound();

        // 2) Get dose events
        var doseResponse = await _client.GetFromJsonAsync<ODataListResponse<DoseEventDto>>(
            $"/odata/DoseEvents?$filter=CycleId eq {id}&$expand=Compound"
        );

        var doses = doseResponse?.Value ?? new List<DoseEventDto>();

        // 3) Get PK data
        var pk = await _client.GetFromJsonAsync<List<PkPointDto>>($"/api/pk/cycle/{id}");

        ViewBag.PkData = pk;
        ViewBag.Cycle = cycle;

        return View(doses);
    }

}