using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.API.Models.RequestModels;

public class LempelZiv78CompressionAndHamming74RequestModel
{
    [Required]
    public string? Base64String { get; set; }
}
