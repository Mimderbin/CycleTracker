using System.ComponentModel.DataAnnotations;

namespace WebUI.Models;

public class DoseEventCreateViewModel
{
    [Required]
    public int CycleId { get; set; }

    [Required]
    [Display(Name = "Compound")]
    public int CompoundId { get; set; }

    [Required]
    [Range(0.1, 10000)]
    [Display(Name = "Amount (mg)")]
    public double AmountMg { get; set; }

    [Required]
    [Display(Name = "Date & time")]
    public DateTime Timestamp { get; set; }

    // for dropdown
    public List<CompoundDto> Compounds { get; set; } = new();
}
