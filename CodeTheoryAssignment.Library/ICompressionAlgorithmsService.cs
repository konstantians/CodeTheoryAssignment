﻿using System.Collections.Specialized;

namespace CodeTheoryAssignment.Library;

public interface ICompressionAlgorithmsService
{
    (string, OrderedDictionary) ArithmeticCodingCompression(string inputBinaryString);
    string ArithmeticCodingDecompression(string compressedBinaryString, OrderedDictionary binaryCharactersIntervals);
    (string, Dictionary<string, string>) HuffmanCompression(string inputBinaryString);
    string HuffmanOrShannonFanoDecompression(Dictionary<string, string> huffmanCodes, string compressedBinaryString);
    string LempelZiv78Compression(string inputBinaryString);
    string LempelZiv78Decompression(string compressedBinaryString);
    (string, Dictionary<string, string>) ShannonFanoCompression(string inputBinaryString);
}