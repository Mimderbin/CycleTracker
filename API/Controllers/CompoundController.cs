using API.Data;
using API.Models;
using Microsoft.AspNetCore.OData.Deltas;

namespace API.Controllers;

public class CompoundsController : CrudController<Compound>
{
    public CompoundsController(AppDbContext context) : base(context) { }

    protected override Task OnBeforeCreate(Compound compound)
    {
        if (string.IsNullOrWhiteSpace(compound.Name))
            throw new Exception("Compound name is required.");

        if (compound.HalfLifeDays <= 0)
            throw new Exception("HalfLifeDays must be greater than 0.");

        return Task.CompletedTask;
    }

    protected override Task OnBeforeUpdate(Compound compound, Delta<Compound> patch)
    {
        // If name is being changed, enforce not empty
        if (patch.GetChangedPropertyNames().Contains(nameof(Compound.Name)))
        {
            if (patch.TryGetPropertyValue(nameof(Compound.Name), out var nameObj))
            {
                var name = nameObj as string;
                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("Compound name cannot be empty.");
            }
        }

        // If half-life is changed, enforce > 0
        if (patch.GetChangedPropertyNames().Contains(nameof(Compound.HalfLifeDays)))
        {
            if (patch.TryGetPropertyValue(nameof(Compound.HalfLifeDays), out var hlObj) &&
                hlObj is double hl &&
                hl <= 0)
            {
                throw new Exception("HalfLifeDays must be greater than 0.");
            }
        }

        return Task.CompletedTask;
    }
}