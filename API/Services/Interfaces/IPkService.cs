using API.DTOs;

namespace API.Services.Interfaces;

public interface IPkService
{
    Task<IReadOnlyList<DailyLevelResult>> GetLevelsForCycleAsync(int cycleId, CancellationToken ct);
}
