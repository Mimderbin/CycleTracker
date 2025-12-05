using API.Data;
using API.DTOs;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class PkService : IPkService
{
    private readonly AppDbContext _context;

    public PkService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DailyLevelResult>> GetLevelsForCycleAsync(int cycleId, CancellationToken ct)
    {
        var cycle = await _context.Cycles
            .Include(c => c.DoseEvents)
                .ThenInclude(d => d.Compound)
            .FirstOrDefaultAsync(c => c.Id == cycleId, ct);

        if (cycle == null)
            throw new KeyNotFoundException($"Cycle {cycleId} not found.");

        var doses = cycle.DoseEvents
            .OrderBy(d => d.Timestamp)
            .ToList();

        if (doses.Count == 0)
            return Array.Empty<DailyLevelResult>();

        var results = new List<DailyLevelResult>();

        var start = cycle.StartDate.Date;
        // If EndDate is null, go until last dose + some buffer (e.g. 5 half-lives of the longest compound)
        var lastDoseDate = doses.Max(d => d.Timestamp).Date;

        double maxHalfLife = doses.Max(d => d.Compound.HalfLifeDays);
        var bufferDays = (int)Math.Ceiling(maxHalfLife * 5); // 5 half-lives ≈ 97% cleared

        var end = (cycle.EndDate?.Date ?? lastDoseDate.AddDays(bufferDays));

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var perCompound = new Dictionary<string, double>();

            foreach (var dose in doses)
            {
                if (dose.Timestamp.Date > day)
                    continue; // dose is in the future relative to this day

                var halfLife = dose.Compound.HalfLifeDays;
                if (halfLife <= 0)
                    continue; // skip broken data

                var daysElapsed = (day - dose.Timestamp.Date).TotalDays;
                if (daysElapsed < 0)
                    continue;

                var remaining = dose.AmountMg * Math.Pow(0.5, daysElapsed / halfLife);
                if (remaining <= 0.01) // negligible
                    continue;

                var key = dose.Compound.Name;
                if (!perCompound.ContainsKey(key))
                    perCompound[key] = 0;

                perCompound[key] += remaining;
            }

            var total = perCompound.Values.Sum();

            results.Add(new DailyLevelResult
            {
                Date = day,
                TotalMg = total,
                PerCompound = perCompound
            });
        }

        return results;
    }
}
