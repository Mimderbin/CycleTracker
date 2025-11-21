using API.Data;
using API.Models;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class DoseEventsController : CrudController<DoseEvent>
{
    public DoseEventsController(AppDbContext context) : base(context) { }

    protected override async Task OnBeforeCreate(DoseEvent evt)
    {
        if (evt.AmountMg <= 0)
            throw new Exception("AmountMg must be greater than 0.");

        // Basic FK existence checks
        bool cycleExists = await _context.Cycles.AnyAsync(c => c.Id == evt.CycleId);
        if (!cycleExists)
            throw new Exception($"Cycle with id {evt.CycleId} does not exist.");

        bool compoundExists = await _context.Compounds.AnyAsync(c => c.Id == evt.CompoundId);
        if (!compoundExists)
            throw new Exception($"Compound with id {evt.CompoundId} does not exist.");
    }

    protected override async Task OnBeforeUpdate(DoseEvent evt, Delta<DoseEvent> patch)
    {
        // If mg is being changed, ensure > 0
        if (patch.GetChangedPropertyNames().Contains(nameof(DoseEvent.AmountMg)))
        {
            if (patch.TryGetPropertyValue(nameof(DoseEvent.AmountMg), out var mgObj) &&
                mgObj is double mg &&
                mg <= 0)
            {
                throw new Exception("AmountMg must be greater than 0.");
            }
        }

        // If CycleId is being changed, verify target cycle exists
        if (patch.GetChangedPropertyNames().Contains(nameof(DoseEvent.CycleId)))
        {
            if (patch.TryGetPropertyValue(nameof(DoseEvent.CycleId), out var cycleObj) &&
                cycleObj is int newCycleId)
            {
                bool cycleExists = await _context.Cycles.AnyAsync(c => c.Id == newCycleId);
                if (!cycleExists)
                    throw new Exception($"Cycle with id {newCycleId} does not exist.");
            }
        }

        // If CompoundId is being changed, verify target compound exists
        if (patch.GetChangedPropertyNames().Contains(nameof(DoseEvent.CompoundId)))
        {
            if (patch.TryGetPropertyValue(nameof(DoseEvent.CompoundId), out var cmpObj) &&
                cmpObj is int newCompoundId)
            {
                bool compoundExists = await _context.Compounds.AnyAsync(c => c.Id == newCompoundId);
                if (!compoundExists)
                    throw new Exception($"Compound with id {newCompoundId} does not exist.");
            }
        }
    }
}
