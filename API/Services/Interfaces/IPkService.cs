using API.DTOs;

namespace API.Services;

public interface IPkService
{
    Task<IReadOnlyList<DailyLevelResult>> GetLevelsForCycleAsync(int cycleId, CancellationToken ct);
}
