namespace WebUI.Models;

public class DoseEventDto
{
    public int Id { get; set; }
    public int CycleId { get; set; }
    public int CompoundId { get; set; }
    public string CompoundName { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public double AmountMg { get; set; }
}