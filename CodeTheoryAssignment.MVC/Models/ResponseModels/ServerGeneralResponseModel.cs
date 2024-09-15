namespace CodeTheoryAssignment.MVC.Models.ResponseModels;

public class ServerGeneralResponseModel
{
    public string? CompressedBinaryRepresentationWithErrorCorrection { get; set; }
    public string? CompressedPaddedBinaryRepresentation { get; set; }
    public string? CompressedBinaryRepresentation { get; set; }
    public string? BinaryRepresentation { get; set; }
    public string? AsciiRepresentation { get; set; }
    public int ErrorsDetected { get; set; }
    public List<int> ErrorPositions { get; set; } = new List<int>();
}
