using System.Text;

namespace CodeTheoryAssignment.Library;

public class ErrorInsertionService : IErrorInsertionService
{
    public string AddOneErrorEvery7Bits(string binarySequence)
    {
        StringBuilder sevenBitSequence = new StringBuilder();
        List<string> binarySequences = new List<string>();
        Random random = new Random(42);
        for (int i = 0; i < binarySequence.Length; i++)
        {
            sevenBitSequence.Append(binarySequence[i]);
            if ((i + 1) % 7 == 0)
            {
                binarySequences.Add(sevenBitSequence.ToString());
                sevenBitSequence.Clear();
            }
        }

        StringBuilder errorBinarySequence = new StringBuilder();
        foreach (string sequence in binarySequences)
        {
            StringBuilder newSequence = new StringBuilder();
            int errorPosition = random.Next(0, 7);
            for (int i = 0; i < sequence.Length; i++)
            {
                if (errorPosition == i)
                {
                    char opossiteCharacter = sequence[i] == '0' ? '1' : '0';
                    newSequence.Append(opossiteCharacter.ToString());
                }
                else
                    newSequence.Append(sequence[i].ToString());
            }

            errorBinarySequence.Append(newSequence.ToString());
        }

        return errorBinarySequence.ToString();
    }

    public (int, string) AddErrorsBasedOnProbability(string binarySequence, double probability)
    {
        Random random = new Random();

        StringBuilder errorBinarySequence = new StringBuilder();
        int errorCount = 0;
        foreach (char bit in binarySequence)
        {
            double randomDouble = random.NextDouble();
            char opossiteValueOfBit = bit == '0' ? '1' : '0';
            char newBitValue = randomDouble <= probability ? opossiteValueOfBit : bit;

            if(opossiteValueOfBit == newBitValue)
                errorCount++;

            errorBinarySequence.Append(newBitValue);
        }

        return (errorCount, errorBinarySequence.ToString());
    }
}
