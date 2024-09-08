using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CodeTheoryAssignment.Library;

public class EncodingAlgorithmsService : IEncodingAlgorithmsService
{
    public EncodingAlgorithmsService()
    {
        Base64ToASCIICharTable = ASCIICharToBase64CharTable.ToDictionary(kv => kv.Value, kv => kv.Key);
        BinaryToAsciiTable = AsciiTable.ToDictionary(kv => kv.Value, kv => kv.Key);
    }

    public string ConvertASCIIToBinaryAlgorithm(string inputString, bool addNullCharacterAddEndOfSequence = true)
    {
        StringBuilder binaryString = new StringBuilder();

        foreach (char character in inputString)
        {
            AsciiTable.TryGetValue(character, out string? binaryValue);
            binaryString.Append(binaryValue);
            binaryString.Append(" ");
        }
        binaryString.Remove(binaryString.Length - 1, 1);
        if(addNullCharacterAddEndOfSequence)
            binaryString.Append(" 00000000");

        return binaryString.ToString();
    }

    public string ConvertBinaryToASCIIAlgorithm(string binarySequence)
    {
        string[] binaryCharacters = binarySequence.Split(" ");
        StringBuilder finalString = new StringBuilder();

        foreach (string binaryCharacter in binaryCharacters)
        {
            if (binaryCharacter == "00000000")
                break;

            BinaryToAsciiTable.TryGetValue(binaryCharacter, out char character);
            finalString.Append(character);
        }

        return finalString.ToString();
    }

    public string AddPaddingForHammingCodeEncoding(string binarySequence)
    {
        Random random = new Random(42);

        //initially make sure that the binary sequence is divisible by 4
        int paddingSymbolsCount = 4 - (binarySequence.Count() % 4) != 4 ? 4 - (binarySequence.Count() % 4) : 0;

        StringBuilder newPaddedBinarySequence = new StringBuilder();
        newPaddedBinarySequence.Append(binarySequence);
        for (int i = 0; i < paddingSymbolsCount; i++)
            newPaddedBinarySequence.Append(random.Next(0, 2));

        // the extra bits that hamming encoding is going to add.
        // The 8 is added here, because of the first byte that shows the length of the extra bits
        int hammingCodeExtraBitsThatWillBeAdded = (newPaddedBinarySequence.Length + 8) / 4 * 3;
        int currentTotalBits = newPaddedBinarySequence.Length + hammingCodeExtraBitsThatWillBeAdded;
        
        //add 7 bits each time(3 will be added automatically)
        int counter = 0;
        int multiplier = 0;
        while ((currentTotalBits + counter + (multiplier * 3)) % 8 != 0)
        {
            counter += 4;
            multiplier++;
        }

        //int totalBits = currentTotalBits + counter;
        int totalBits = newPaddedBinarySequence.Length + counter;
        for (int i = newPaddedBinarySequence.Length; i < totalBits; i++)
            newPaddedBinarySequence.Append(random.Next(0, 2));

        int totalAddedBits = paddingSymbolsCount + counter;
        string totalAddedBitsInBinary = Convert.ToString(totalAddedBits, 2);
        while (totalAddedBitsInBinary.Length != 8)
            totalAddedBitsInBinary = "0" + totalAddedBitsInBinary; 

        string binaryPaddedString = totalAddedBitsInBinary + newPaddedBinarySequence.ToString();

        return binaryPaddedString;
    }

    public string RemovePaddingFromHammingCodeEncoding(string paddedBinarySequence)
    {
        string paddingLengthString = paddedBinarySequence.Substring(0, 8);
        int paddingLength = Convert.ToInt32(paddingLengthString, 2);

        string binarySequenceWithoutPadding = paddedBinarySequence.Remove(paddedBinarySequence.Length - paddingLength, paddingLength);
        string binarySequence = binarySequenceWithoutPadding.Remove(0, 8);

        return binarySequence;
    }

    public string AddPaddingToBinarySequence(string binarySequence)
    {
        Random random = new Random(42);

        int paddingSymbolsCount = 8 - (binarySequence.Count() % 8) != 8 ? 8 - (binarySequence.Count() % 8) : 0;

        string paddingLength = ConvertASCIIToBinaryAlgorithm(paddingSymbolsCount.ToString(), false);
        
        StringBuilder newPaddedBinarySequence = new StringBuilder();
        newPaddedBinarySequence.Append(paddingLength);
        newPaddedBinarySequence.Append(binarySequence);
        for (int i = 0; i < paddingSymbolsCount; i++) 
            newPaddedBinarySequence.Append(random.Next(0, 2));

        return newPaddedBinarySequence.ToString();
    }

    public string RemovePaddingFromBinarySequence(string paddedBinarySequence)
    {
        string paddingLengthString = paddedBinarySequence.Substring(0, 8);
        int paddingLength = Int32.Parse(ConvertBinaryToASCIIAlgorithm(paddingLengthString));

        string binarySequenceWithoutPadding = paddedBinarySequence.Remove(paddedBinarySequence.Length - paddingLength, paddingLength);
        string binarySequence = binarySequenceWithoutPadding.Remove(0, 8); 
        
        return binarySequence;
    }

    public string ConvertBinaryToBase64Algorithm(string binarySequence)
    {
        string binarySequenceWithoutSpaces = binarySequence.Replace(" ", "");

        StringBuilder base64StringBuilder = new StringBuilder();
        StringBuilder subBase64Character = new StringBuilder();

        for(int i = 0; i < binarySequenceWithoutSpaces.Length; i++)
        {
            subBase64Character.Append(binarySequenceWithoutSpaces[i]);

            if ((i + 1) % 6 == 0)
            {
                Base64ToASCIICharTable.TryGetValue(subBase64Character.ToString(), out string? character);
                base64StringBuilder.Append(character);
                subBase64Character.Clear();
            }
            //case where the binary sequence is not divisible by 6(add 0s if needed)
            else if ((i + 1) % 6 != 0 && i == binarySequenceWithoutSpaces.Length - 1)
            {
                for (int j = (i + 1) % 6; j < 6; j++)
                    subBase64Character.Append("0");

                Base64ToASCIICharTable.TryGetValue(subBase64Character.ToString(), out string? character);
                base64StringBuilder.Append(character);
            }
        }

        string base64String = base64StringBuilder.ToString();
        //In the end the bytes need to be divisible by 4
        int remainder = base64String.Count() / 8 % 4;

        //if not add "="/00111101 for padding
        for (int i = 0; i < remainder; i++)
            base64String += "00111101";

        return base64String;
    }

    public string ConvertBase64ToBinaryAlgorithm(string base64Sequence)
    {
        StringBuilder binarySequenceBuilder = new StringBuilder();
        StringBuilder byteStringBuilder = new StringBuilder();

        for (int i = 0; i < base64Sequence.Length; i++)
        {
            byteStringBuilder.Append(base64Sequence[i]);
            if (byteStringBuilder.ToString() == "00111101")
                break;

            if ((i + 1) % 8 == 0)
            {
                ASCIICharToBase64CharTable.TryGetValue(byteStringBuilder.ToString(), out string? character);
                binarySequenceBuilder.Append(character);
                byteStringBuilder.Clear();
            }
        }

        string binarySequence = binarySequenceBuilder.ToString();
        int addedZerosAtEndCount = binarySequence.Length % 8;
        binarySequence = binarySequence.Remove(binarySequence.Length - addedZerosAtEndCount, addedZerosAtEndCount);

        return binarySequence;
    }

    internal Dictionary<string, string> ASCIICharToBase64CharTable { get; set; } = new Dictionary<string, string>(){
        {"01000001", "000000"},{"01000010", "000001"},{"01000011", "000010"},{"01000100", "000011"},
        {"01000101", "000100"},{"01000110", "000101"},{"01000111", "000110"},{"01001000", "000111"},
        {"01001001", "001000"},{"01001010", "001001"},{"01001011", "001010"},{"01001100", "001011"},
        {"01001101", "001100"},{"01001110", "001101"},{"01001111", "001110"},{"01010000", "001111"},
        {"01010001", "010000"},{"01010010", "010001"},{"01010011", "010010"},{"01010100", "010011"},
        {"01010101", "010100"},{"01010110", "010101"},{"01010111", "010110"},{"01011000", "010111"},
        {"01011001", "011000"},{"01011010", "011001"},{"01100001", "011010"},{"01100010", "011011"},
        {"01100011", "011100"},{"01100100", "011101"},{"01100101", "011110"},{"01100110", "011111"},
        {"01100111", "100000"},{"01101000", "100001"},{"01101001", "100010"},{"01101010", "100011"},
        {"01101011", "100100"},{"01101100", "100101"},{"01101101", "100110"},{"01101110", "100111"},
        {"01101111", "101000"},{"01110000", "101001"},{"01110001", "101010"},{"01110010", "101011"},
        {"01110011", "101100"},{"01110100", "101101"},{"01110101", "101110"},{"01110110", "101111"},
        {"01110111", "110000"},{"01111000", "110001"},{"01111001", "110010"},{"01111010", "110011"},
        {"00110000", "110100"},{"00110001", "110101"},{"00110010", "110110"},{"00110011", "110111"}, //in this row start numbers
        {"00110100", "111000"},{"00110101", "111001"},{"00110110", "111010"},{"00110111", "111011"},
        {"00111000", "111100"},{"00111001", "111101"},{"00101011", "111110"},{"00101111", "111111"}  //last 2 are + and /
    };

    internal Dictionary<string, string> Base64ToASCIICharTable { get; set; }

    internal Dictionary<char, string> AsciiTable { get; set; } = new Dictionary<char, string>(){
        {'\0', "00000000"},
        {' ', "00100000"},{'!', "00100001"},{'"', "00100010"},{'#', "00100011"},
        {'$', "00100100"},{'%', "00100101"},{'&', "00100110"},{'\'', "00100111"},
        {'(', "00101000"},{')', "00101001"},{'*', "00101010"},{'+', "00101011"},
        {',', "00101100"},{'-', "00101101"},{'.', "00101110"},{'/', "00101111"},
        {'0', "00110000"},{'1', "00110001"},{'2', "00110010"},{'3', "00110011"},
        {'4', "00110100"},{'5', "00110101"},{'6', "00110110"},{'7', "00110111"},
        {'8', "00111000"},{'9', "00111001"},{':', "00111010"},{';', "00111011"},
        {'<', "00111100"},{'=', "00111101"},{'>', "00111110"},{'?', "00111111"},
        {'@', "01000000"},{'A', "01000001"},{'B', "01000010"},{'C', "01000011"},
        {'D', "01000100"},{'E', "01000101"},{'F', "01000110"},{'G', "01000111"},
        {'H', "01001000"},{'I', "01001001"},{'J', "01001010"},{'K', "01001011"},
        {'L', "01001100"},{'M', "01001101"},{'N', "01001110"},{'O', "01001111"},
        {'P', "01010000"},{'Q', "01010001"},{'R', "01010010"},{'S', "01010011"},
        {'T', "01010100"},{'U', "01010101"},{'V', "01010110"},{'W', "01010111"},
        {'X', "01011000"},{'Y', "01011001"},{'Z', "01011010"},{'[', "01011011"},
        {'\\', "01011100"},{']', "01011101"},{'^', "01011110"},{'_', "01011111"},
        {'`', "01100000"},{'a', "01100001"},{'b', "01100010"},{'c', "01100011"},
        {'d', "01100100"},{'e', "01100101"},{'f', "01100110"},{'g', "01100111"},
        {'h', "01101000"},{'i', "01101001"},{'j', "01101010"},{'k', "01101011"},
        {'l', "01101100"},{'m', "01101101"},{'n', "01101110"},{'o', "01101111"},
        {'p', "01110000"},{'q', "01110001"},{'r', "01110010"},{'s', "01110011"},
        {'t', "01110100"},{'u', "01110101"},{'v', "01110110"},{'w', "01110111"},
        {'x', "01111000"},{'y', "01111001"},{'z', "01111010"},{'{', "01111011"},
        {'|', "01111100"},{'}', "01111101"},{'~', "01111110"}
    };

    internal Dictionary<string, char> BinaryToAsciiTable { get; set; }
}
