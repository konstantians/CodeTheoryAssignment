using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.API.Models.RequestModels;

public class LempelZiv78CompressionAndCrcRequestModel
{
    [Required]
    public Dictionary<int, string> Base64PositionsAndStrings { get; set; } = new Dictionary<int, string>();
}
