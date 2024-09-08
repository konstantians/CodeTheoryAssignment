using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.MVC.Models;

public class AlgorithmModel
{
    [Required]
    public string? UserInput { get; set; }
    public string? CompressionAlgorithm { get; set; }
    public string? ErrorCorrectingAlgorithm { get; set; }
}
