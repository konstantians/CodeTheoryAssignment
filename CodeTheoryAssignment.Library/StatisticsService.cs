namespace CodeTheoryAssignment.Library;

public class StatisticsService : IStatisticsService
{
    public double CalculateCompressionRate(int uncompressedBitCount, int compressedBitCount)
    {
        double uncompressedByteCount = uncompressedBitCount / 8;
        double compressedByteCount = compressedBitCount / 8;
        return Math.Round((double)(uncompressedByteCount - compressedByteCount) / uncompressedByteCount * 100, 2);
    }

    public double CalculateEntropy(string text)
    {
        var frequencyDictictionary = new Dictionary<char, int>();

        // count frequencies for each character
        foreach (char character in text)
        {
            if (frequencyDictictionary.ContainsKey(character))
                frequencyDictictionary[character]++;
            else
                frequencyDictictionary[character] = 1;
        }

        double entropy = 0.0;
        foreach (var characterAndProbability in frequencyDictictionary)
        {
            double probability = characterAndProbability.Value / (double)text.Length;
            entropy -= probability * Math.Log2(probability);
        }

        return Math.Round(entropy, 2);
    }
}
