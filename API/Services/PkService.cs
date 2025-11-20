using API.Models;

namespace API.Services;
public class PkService
{
    public IEnumerable<DailyLevelResult> CalculateLevels(Cycle cycle)
    {
        var doses = cycle.DoseEvents.OrderBy(d => d.Timestamp).ToList();
        if (doses.Count == 0) yield break;

        DateTime start = cycle.StartDate;
        DateTime end = cycle.EndDate ?? DateTime.Now;

        for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
        {
            var perCompound = new Dictionary<string, double>();

            foreach (var dose in doses)
            {
                if (dose.Timestamp.Date > day) continue;

                var halfLife = dose.Compound.HalfLifeDays;
                var daysElapsed = (day - dose.Timestamp.Date).TotalDays;
                var remaining = dose.AmountMg * Math.Pow(0.5, daysElapsed / halfLife);

                if (!perCompound.ContainsKey(dose.Compound.Name))
                    perCompound[dose.Compound.Name] = 0;

                perCompound[dose.Compound.Name] += remaining;
            }

            yield return new DailyLevelResult
            {
                Date = day,
                TotalMg = perCompound.Values.Sum(),
                PerCompound = perCompound
            };
        }
    }
}

public class DailyLevelResult
{
    public DateTime Date { get; set; }
    public double TotalMg { get; set; }
    public Dictionary<string, double> PerCompound { get; set; } = new();
}
