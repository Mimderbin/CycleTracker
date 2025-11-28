namespace API.Models;

public class Compound
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Ester { get; set; }
    public double HalfLifeDays { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<DoseEvent>? DoseEvents { get; set; } = new List<DoseEvent>();
}
    