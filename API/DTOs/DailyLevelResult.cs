namespace API.DTOs;

public class DailyLevelResult
{
    public DateTime Date { get; set; }
    public double TotalMg { get; set; }
    public Dictionary<string, double> PerCompound { get; set; } = new();
}
