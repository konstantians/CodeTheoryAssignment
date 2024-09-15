using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.MVC.Models.RequestModels;

public class UILempelZiv78CompressionAndCrcRequestModel
{

    [Required]
    public Dictionary<int, string> Base64PositionsAndStrings { get; set; } = new Dictionary<int, string>();
}
