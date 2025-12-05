namespace WebUI.Models;

public class CycleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }
}
