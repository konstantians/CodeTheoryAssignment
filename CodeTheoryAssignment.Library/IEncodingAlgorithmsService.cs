namespace CodeTheoryAssignment.Library;

public interface IEncodingAlgorithmsService
{
    string ConvertASCIIToBinaryAlgorithm(string inputString, bool addNullCharacterAddEndOfSequence = true);
    string ConvertBinaryToBase64Algorithm(string binarySequence);
    string ConvertBinaryToASCIIAlgorithm(string binarySequence);
    string AddPaddingToBinarySequence(string binarySequence);
    string RemovePaddingFromBinarySequence(string paddedBinarySequence);
    string ConvertBase64ToBinaryAlgorithm(string AsciiSequence);
    string AddPaddingForHammingCodeEncoding(string binarySequence);
    string RemovePaddingFromHammingCodeEncoding(string paddedBinarySequence);
    string SplitBinarySequenceIntoBytes(string binarySequence);
}