namespace API.Models;

public class DoseEvent
{
    public int Id { get; set; }
    public int CycleId { get; set; }
    public int CompoundId { get; set; }
    public DateTime Timestamp { get; set; }
    public double AmountMg { get; set; }
    public string? Route { get; set; } // IM, Oral, etc.
    public string? Notes { get; set; }

    // Navigation
    public Cycle? Cycle { get; set; } = null!;
    public Compound? Compound { get; set; } = null!;
}
