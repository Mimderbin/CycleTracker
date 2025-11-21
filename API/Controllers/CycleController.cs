using API.Data;
using API.Models;
using Microsoft.AspNetCore.OData.Deltas;

namespace API.Controllers;

public class CyclesController : CrudController<Cycle>
{
    public CyclesController(AppDbContext context) : base(context) { }

    protected override Task OnBeforeCreate(Cycle cycle)
    {
        if (cycle.EndDate.HasValue && cycle.EndDate.Value < cycle.StartDate)
            throw new Exception("EndDate cannot be before StartDate.");

        if (string.IsNullOrWhiteSpace(cycle.Name))
            throw new Exception("Cycle name is required.");

        return Task.CompletedTask;
    }

    protected override Task OnBeforeUpdate(Cycle cycle, Delta<Cycle> patch)
    {
        // We only care if StartDate or EndDate is being patched
        var changed = patch.GetChangedPropertyNames().ToHashSet();

        DateTime start = cycle.StartDate;
        DateTime? end = cycle.EndDate;

        if (changed.Contains(nameof(Cycle.StartDate)))
        {
            if (patch.TryGetPropertyValue(nameof(Cycle.StartDate), out var startObj) &&
                startObj is DateTime newStart)
            {
                start = newStart;
            }
        }

        if (changed.Contains(nameof(Cycle.EndDate)))
        {
            if (patch.TryGetPropertyValue(nameof(Cycle.EndDate), out var endObj))
            {
                end = endObj as DateTime?;
            }
        }

        if (end.HasValue && end.Value < start)
            throw new Exception("EndDate cannot be before StartDate.");

        return Task.CompletedTask;
    }
}