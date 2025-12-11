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
    
    [HttpGet]
    public async Task<IActionResult> AddDose(int CycleId)
    {
        // get cycle just for context (title etc.)
        var cycle = await _client.GetFromJsonAsync<CycleDto>($"/odata/Cycles({CycleId})");

        if (cycle == null)
            return NotFound();

        // get list of compounds for dropdown
        var compoundsResponse =
            await _client.GetFromJsonAsync<ODataListResponse<CompoundDto>>("/odata/Compounds");

        var vm = new DoseEventCreateViewModel
        {
            CycleId = CycleId,
            Timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
            Compounds = compoundsResponse?.Value ?? new List<CompoundDto>()
        };

        ViewBag.Cycle = cycle;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddDose(DoseEventCreateViewModel vm)
    {
        Console.WriteLine("=== AddDose POST HIT ===");
        Console.WriteLine($"ModelState Valid: {ModelState.IsValid}");
        Console.WriteLine($"Timestamp Raw: {vm.Timestamp:o} (Kind: {vm.Timestamp.Kind})");

        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Console.WriteLine($"ModelState error on '{entry.Key}': {error.ErrorMessage}");
            }
        }


        // MVC datetime-local sends "unspecified". 
        // Make SURE it's unspecified no matter what.
        vm.Timestamp = DateTime.SpecifyKind(vm.Timestamp, DateTimeKind.Unspecified);

        if (!ModelState.IsValid)
        {
            // ALSO ensure unspecified inside validation-fail flow
            vm.Timestamp = DateTime.SpecifyKind(vm.Timestamp, DateTimeKind.Unspecified);

            // Rebuild dropdown + cycle info
            var cycle = await _client.GetFromJsonAsync<CycleDto>($"/odata/Cycles({vm.CycleId})");
            var compoundsResponse =
                await _client.GetFromJsonAsync<ODataListResponse<CompoundDto>>("/odata/Compounds");

            vm.Compounds = compoundsResponse?.Value ?? new List<CompoundDto>();
            ViewBag.Cycle = cycle;

            return View(vm);
        }

        var payload = new
        {
            Id = 0,
            CycleId = vm.CycleId,
            CompoundId = vm.CompoundId,
            Timestamp = vm.Timestamp,
            AmountMg = vm.AmountMg,
            Route = (string?)null,
            Notes = (string?)null
        };



        // Actual API call
        var response = await _client.PostAsJsonAsync("/odata/DoseEvents", payload);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", $"API error: {response.StatusCode}");

            // ALSO fix Timestamp here (important)
            vm.Timestamp = DateTime.SpecifyKind(vm.Timestamp, DateTimeKind.Unspecified);

            var cycle = await _client.GetFromJsonAsync<CycleDto>($"/odata/Cycles({vm.CycleId})");
            var compoundsResponse =
                await _client.GetFromJsonAsync<ODataListResponse<CompoundDto>>("/odata/Compounds");

            vm.Compounds = compoundsResponse?.Value ?? new List<CompoundDto>();
            ViewBag.Cycle = cycle;

            return View(vm);
        }

        // Success → back to cycle details
        return RedirectToAction("Details", new { id = vm.CycleId });
    }
}