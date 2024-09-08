using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.API.Models;

public class HuffmanCompressionAndHamming74RequestModel
{
    [Required]
    public Dictionary<string, string> HuffmanCodes { get; set; } = new Dictionary<string, string>();
    [Required]
    public string? Base64String { get; set; }
}
