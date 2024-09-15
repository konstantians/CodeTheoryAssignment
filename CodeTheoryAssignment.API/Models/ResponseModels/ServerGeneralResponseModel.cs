namespace CodeTheoryAssignment.API.Models.ResponseModels;

public class ServerGeneralResponseModel
{
    public string? CompressedBinaryRepresentationWithErrorCorrection { get; set; }
    public string? CompressedPaddedBinaryRepresentation { get; set; }
    public string? CompressedBinaryRepresentation { get; set; }
    public string? BinaryRepresentation { get; set; }
    public string? AsciiRepresentation { get; set; }
    public int ErrorsDetected { get; set; }
    public List<int> ErrorPositions { get; set; } = new List<int>();

    public ServerGeneralResponseModel(string? compressedBinaryRepresentationWithErrorCorrection, string? compressedPaddedBinaryRepresentation, string? compressedBinaryRepresentation, 
        string? binaryRepresentation, string? asciiRepresentation, int errorsDetected)
    {
        CompressedBinaryRepresentationWithErrorCorrection = compressedBinaryRepresentationWithErrorCorrection;
        CompressedPaddedBinaryRepresentation = compressedPaddedBinaryRepresentation;
        CompressedBinaryRepresentation = compressedBinaryRepresentation;
        BinaryRepresentation = binaryRepresentation;
        AsciiRepresentation = asciiRepresentation;
        ErrorsDetected = errorsDetected;
    }

    public ServerGeneralResponseModel(string? compressedBinaryRepresentationWithErrorCorrection, string? compressedPaddedBinaryRepresentation, string? compressedBinaryRepresentation,
        string? binaryRepresentation, string? asciiRepresentation, List<int> errorPositions)
    {
        CompressedBinaryRepresentationWithErrorCorrection = compressedBinaryRepresentationWithErrorCorrection;
        CompressedPaddedBinaryRepresentation = compressedPaddedBinaryRepresentation;
        CompressedBinaryRepresentation = compressedBinaryRepresentation;
        BinaryRepresentation = binaryRepresentation;
        AsciiRepresentation = asciiRepresentation;
        ErrorPositions = errorPositions;
    }

    public ServerGeneralResponseModel(string? compressedBinaryRepresentationWithErrorCorrection, string? compressedPaddedBinaryRepresentation, string? compressedBinaryRepresentation,
        string? binaryRepresentation, string? asciiRepresentation, int errorsDetected, List<int> errorPositions)
    {
        CompressedBinaryRepresentationWithErrorCorrection = compressedBinaryRepresentationWithErrorCorrection;
        CompressedPaddedBinaryRepresentation = compressedPaddedBinaryRepresentation;
        CompressedBinaryRepresentation = compressedBinaryRepresentation;
        BinaryRepresentation = binaryRepresentation;
        AsciiRepresentation = asciiRepresentation;
        ErrorsDetected = errorsDetected;
        ErrorPositions = errorPositions;
    }
}
