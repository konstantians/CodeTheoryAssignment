using System.Text;

namespace CodeTheoryAssignment.Library;

public class ErrorCorrectingAlgorithmsService : IErrorCorrectingAlgorithmsService
{
    public string RetransmissionMechanism(string binarySequence, List<int> correspondingPositionOfCharacter)
    {
        List<string> binaryStrings = new List<string>();
        StringBuilder byteStringBuilder = new StringBuilder();
        StringBuilder BinarySequenceThatNeedsToBeRetransmitted = new StringBuilder();
        for (int i = 0; i < binarySequence.Length; i++)
        {
            byteStringBuilder.Append(binarySequence[i]);
            if ((i + 1) % 8 == 0)
            {
                //Add the byte padded by k = n - 1 bits, where n is the errorCount of bits of the generator polynomial
                binaryStrings.Add(byteStringBuilder.ToString());
                if (!correspondingPositionOfCharacter.Contains(i))
                    BinarySequenceThatNeedsToBeRetransmitted.Append(byteStringBuilder.ToString());

                byteStringBuilder.Clear();
            }
        }

        return BinarySequenceThatNeedsToBeRetransmitted.ToString();
    }

    public string CyclicRedundancyCodeEncoding(string binarySequence)
    {
        string generatorPolynomial = "100000111"; //x**8 + x**2 + x + 1 for CRC-8  

        List<string> binaryStrings = new List<string>();
        StringBuilder byteStringBuilder = new StringBuilder();
        for (int i = 0; i < binarySequence.Length; i++)
        {
            byteStringBuilder.Append(binarySequence[i]);
            if ((i + 1) % 8 == 0)
            {
                //Add the byte padded by k = n - 1 bits, where n is the errorCount of bits of the generator polynomial
                binaryStrings.Add(byteStringBuilder.ToString() + "00000000");
                byteStringBuilder.Clear();
            }
        }

        StringBuilder crcEncodedSequence = new StringBuilder();
        foreach (string binaryString in binaryStrings)
        {
            string remainder = PolynomialDivisionOperation(binaryString, generatorPolynomial);

            crcEncodedSequence.Append(binaryString.Substring(0, binaryString.Length - 8) + remainder);
        }

        return crcEncodedSequence.ToString();
    }
    
    public (string, List<int>) CyclicRedundancyCodeDecoding(string crcEncodedSequence)
    {

        string generatorPolynomial = "100000111"; //x**8 + x**2 + x + 1 for CRC-8  

        List<string> binaryCrcEncodedStrings = new List<string>();
        StringBuilder byteStringBuilder = new StringBuilder();
        for (int i = 0; i < crcEncodedSequence.Length; i++)
        {
            byteStringBuilder.Append(crcEncodedSequence[i]);
            if ((i + 1) % 16 == 0)
            {
                //Add the byte padded by k = n - 1 bits, where n is the errorCount of bits of the generator polynomial
                binaryCrcEncodedStrings.Add(byteStringBuilder.ToString());
                byteStringBuilder.Clear();
            }
        }

        List<int> errorBytePositions = new List<int>();
        StringBuilder binaryStringSequence = new StringBuilder();
        int index = 0;
        foreach (string binaryCrcEncodedString in binaryCrcEncodedStrings)
        {
            string remainder = PolynomialDivisionOperation(binaryCrcEncodedString, generatorPolynomial);

            if (remainder != "00000000")
                errorBytePositions.Add(index);

            binaryStringSequence.Append(binaryCrcEncodedString.Substring(0, 8));
            index++;
        }

        return (binaryStringSequence.ToString(), errorBytePositions);
    }
    private string PolynomialDivisionOperation(string dividend, string generatorPolynomial)
    {
        string binaryStringCopy = dividend;
        string startingPoint = binaryStringCopy.Substring(0, 9); //starting point has the first 9 bits out of the total 16 bits
        binaryStringCopy = binaryStringCopy.Remove(0, 9); //binaryStringcopy has the last 7 bits out of the total 16 bits

        while (binaryStringCopy.Length > 0)
        {
            //Do the division if the starting bit of startingPoint, which is what is left, starts with 1 with else skip the division
            if (!startingPoint.StartsWith("0"))
                startingPoint = XorBinaryStrings(startingPoint, generatorPolynomial);

            startingPoint = startingPoint.Remove(0, 1);
            startingPoint += binaryStringCopy[0];
            binaryStringCopy = binaryStringCopy.Remove(0, 1);
        }

        if (!startingPoint.StartsWith("0"))
            startingPoint = XorBinaryStrings(startingPoint, generatorPolynomial);

        startingPoint = startingPoint.Remove(0, 1);
        return startingPoint;
    }
    private string XorBinaryStrings(string binaryString1, string binaryString2)
    {
        //for the xor to work they must have the same length
        if(binaryString1.Length != binaryString2.Length)
            throw new Exception("binary strings do not have the same length");

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < binaryString1.Length; i++)
            result.Append(binaryString1[i] == binaryString2[i] ? '0' : '1'); //Essentially XOR

        return result.ToString();
    }

    public string HammingCode74Encoding(string binarySequence)
    {
        List<string> binaryStrings = new List<string>();
        StringBuilder nibble = new StringBuilder();
        for(int i = 0; i < binarySequence.Length; i++)
        {
            nibble.Append(binarySequence[i]);
            if ((i + 1) % 4 == 0)
            {
                binaryStrings.Add(nibble.ToString());
                nibble.Clear();
            }
        }

        StringBuilder hammingEncodedBinaryString = new StringBuilder();
        foreach (string binaryString in binaryStrings)
        {
            //parityBit0 = B0 + B1 + B2
            char parityBit0 = (binaryString[0] - '0' + binaryString[1] - '0' + binaryString[2] - '0') % 2 == 0 ? '0' : '1';
            //parityBit1 = B0 + B1 + B3 
            char parityBit1 = (binaryString[0] - '0' + binaryString[1] - '0' + binaryString[3] - '0') % 2 == 0 ? '0' : '1';
            //parityBit2 = B0 + B2 + B3 
            char parityBit2 = (binaryString[0] - '0' + binaryString[2] - '0' + binaryString[3] - '0') % 2 == 0 ? '0' : '1';

            hammingEncodedBinaryString.Append(binaryString[0].ToString() + binaryString[1].ToString() + binaryString[2].ToString() +
                parityBit0.ToString() + binaryString[3].ToString() + parityBit1.ToString() + parityBit2.ToString());
        }

        return hammingEncodedBinaryString.ToString();
    }

    public (string, int) HammingCode74Decoding(string hammingCodeEncodedSequence)
    {
        List<string> hammingCodeEncodedCharacters = new List<string>();
        StringBuilder hammingCodeEncodedCharacter = new StringBuilder();
        for (int i = 0; i < hammingCodeEncodedSequence.Length; i++)
        {
            hammingCodeEncodedCharacter.Append(hammingCodeEncodedSequence[i]);
            if ((i + 1) % 7 == 0)
            {
                hammingCodeEncodedCharacters.Add(hammingCodeEncodedCharacter.ToString());
                hammingCodeEncodedCharacter.Clear();
            }
        }

        StringBuilder binaryString = new StringBuilder();
        int detectedErrors = 0;
        foreach (string encodedCharacter in hammingCodeEncodedCharacters)
        {
            char correctParityBit0;
            char correctParityBit1;
            char correctParityBit2;
            bool hasError = false;

            //parityBit0 = B0 + B1 + B2
            correctParityBit0 = (encodedCharacter[0] - '0' + encodedCharacter[1] - '0' + encodedCharacter[2] - '0') % 2 == 0 ? '0' : '1';
            correctParityBit1 = (encodedCharacter[0] - '0' + encodedCharacter[1] - '0' + encodedCharacter[4] - '0') % 2 == 0 ? '0' : '1';
            correctParityBit2 = (encodedCharacter[0] - '0' + encodedCharacter[2] - '0' + encodedCharacter[4] - '0') % 2 == 0 ? '0' : '1';

            if (encodedCharacter[3] != correctParityBit0)
                hasError = true;
            if (encodedCharacter[5] != correctParityBit1)
                hasError = true;
            else if (encodedCharacter[6] != correctParityBit2)
                hasError = true;

            if (!hasError)
            {
                binaryString.Append(encodedCharacter[0].ToString() + encodedCharacter[1].ToString() + 
                    encodedCharacter[2].ToString() + encodedCharacter[4].ToString());
                continue;
            }

            //if there was error
            correctParityBit0 = encodedCharacter[3] != correctParityBit0 ? '1' : '0';
            correctParityBit1 = encodedCharacter[5] != correctParityBit1 ? '1' : '0';
            correctParityBit2 = encodedCharacter[6] != correctParityBit2 ? '1' : '0';

            string errorLocationInBinary = correctParityBit0.ToString() + correctParityBit1.ToString() + correctParityBit2.ToString();
            int errorLocation = 7 - Convert.ToInt32(errorLocationInBinary, 2);
            string correctedEncodedCharacter = "";
            for(int i = 0; i < encodedCharacter.Length; i++)
            {
                if (i != errorLocation)
                    correctedEncodedCharacter += encodedCharacter[i];
                else
                    correctedEncodedCharacter += encodedCharacter[i] == '1' ? '0' : '1';
            }

            binaryString.Append(correctedEncodedCharacter[0].ToString() + correctedEncodedCharacter[1].ToString() + 
                correctedEncodedCharacter[2].ToString() + correctedEncodedCharacter[4].ToString());

            detectedErrors++;
        }

        return (binaryString.ToString(), detectedErrors);
    }
}
