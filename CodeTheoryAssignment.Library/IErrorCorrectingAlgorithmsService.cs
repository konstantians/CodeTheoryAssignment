namespace CodeTheoryAssignment.Library;

public interface IErrorCorrectingAlgorithmsService
{
    string CyclicRedundancyCodeEncoding(string binarySequence);
    (string,List<int>) CyclicRedundancyCodeDecoding(string crcEncodedSequence);
    string HammingCode74Encoding(string binarySequence);
    (string, int) HammingCode74Decoding(string hammingCodeEncodedSequence);
    string RetransmissionMechanism(string initialBinarySequence, List<int> byteErrorPositions);
}