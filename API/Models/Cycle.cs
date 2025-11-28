namespace API.Models;

public class Cycle
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Type { get; set; } // Bulk, Cruise, Cut, etc.
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<DoseEvent>? DoseEvents { get; set; } = new List<DoseEvent>();
}
