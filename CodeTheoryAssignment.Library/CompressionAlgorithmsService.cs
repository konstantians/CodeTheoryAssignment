using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace CodeTheoryAssignment.Library;

public class CompressionAlgorithmsService : ICompressionAlgorithmsService
{
    internal class Node
    {
        public Node? LeftChild { get; set; }
        public Node? RightChild { get; set; }
        public string? BinaryCharacter { get; set; }
        public double Probability { get; set; }

        public Node()
        {

        }
        public Node(string binaryCharacter, double probability)
        {
            BinaryCharacter = binaryCharacter;
            Probability = probability;
        }
    }
    public (string, Dictionary<string, string>) HuffmanCompression(string inputBinaryString)
    {
        List<string> binaryCharacters = inputBinaryString.Split(" ").ToList();
        List<string> binaryCharacterDeepCopy = new();
        foreach (string character in binaryCharacters)
            binaryCharacterDeepCopy.Add(character);

        PriorityQueue<string, double> charactersProbabilitiesPriorityQueue = CalculateBinaryCharactersProbabilities(binaryCharacterDeepCopy);

        List<Node> currentNodes = new List<Node>();
        while(charactersProbabilitiesPriorityQueue.Count > 1)
        {
            string[] firstCharacterAndProbability = charactersProbabilitiesPriorityQueue.Dequeue().Split(" ");
            string[] secondCharacterAndProbability = charactersProbabilitiesPriorityQueue.Dequeue().Split(" ");
            
            string firstCharacter = firstCharacterAndProbability[0];
            double firstCharacterProbability = Double.Parse(firstCharacterAndProbability[1]);
            string secondCharacter = secondCharacterAndProbability[0];
            double secondCharacterProbability = Double.Parse(secondCharacterAndProbability[1]);

            Node newNode = new Node();
            //if these characters where initials and not added after adding 2 probabilities together, which means that these nodes do not exist
            if (firstCharacter.Length == 8 && secondCharacter.Length == 8)
            {
                newNode.LeftChild = new Node(firstCharacter, firstCharacterProbability);
                newNode.RightChild = new Node(secondCharacter, secondCharacterProbability);
            }
            //that means that left node exists and should become a child
            else if(firstCharacter.Length < 8 && secondCharacter.Length == 8)
            {
                newNode.LeftChild = currentNodes.Find(node => node.BinaryCharacter == firstCharacter)!;
                newNode.RightChild = new Node(secondCharacter, firstCharacterProbability);
                
            }
            //that means that right node exists and should become a child
            else if (firstCharacter.Length == 8 && secondCharacter.Length < 8)
            {
                newNode.LeftChild = new Node(firstCharacter, firstCharacterProbability);
                newNode.RightChild = currentNodes.Find(node => node.BinaryCharacter == secondCharacter)!;
            }
            //both are new nodes/ they both exist, maybe this case is impossible
            else
            {
                newNode.LeftChild = currentNodes.Find(node => node.BinaryCharacter == firstCharacter)!;
                newNode.RightChild = currentNodes.Find(node => node.BinaryCharacter == secondCharacter)!;
            }

            newNode.Probability = Math.Round(firstCharacterProbability + secondCharacterProbability, 5);
            newNode.BinaryCharacter = currentNodes.Count.ToString();
            currentNodes.Add(newNode);
            charactersProbabilitiesPriorityQueue.Enqueue($"{newNode.BinaryCharacter} {newNode.Probability}", newNode.Probability);
        }

        Node root = currentNodes.FirstOrDefault(node => node.Probability >= 0.98 && node.Probability <= 1.1)!;
        StringBuilder compressedBinaryString = new StringBuilder();
        Dictionary<string, string> huffmanCodes = new Dictionary<string, string>();

        // BFS to traverse the tree to the correct node for each character
        foreach (string currentCharacter in binaryCharacters)
        {
            Queue<(Node, string)> queue = new();
            queue.Enqueue((root, ""));
            while (queue.Count > 0)
            {
                (Node currentNode, string currentEncodedValue) = queue.Dequeue();
                if (currentNode.BinaryCharacter == currentCharacter)
                {
                    compressedBinaryString.Append(currentEncodedValue);
                    huffmanCodes.TryAdd(currentEncodedValue, currentCharacter);
                    break;
                }

                if (currentNode.LeftChild != null)
                    queue.Enqueue((currentNode.LeftChild, currentEncodedValue + "0"));
                if (currentNode.RightChild != null)
                    queue.Enqueue((currentNode.RightChild, currentEncodedValue + "1"));
            }
        }

        return (compressedBinaryString.ToString(), huffmanCodes);
    }
    public string HuffmanDecompression(Dictionary<string, string> huffmanCodes, string compressedBinaryString)
    {
        StringBuilder finalBinaryString = new StringBuilder();
        StringBuilder currentCharacter = new StringBuilder();
        foreach (char compressedBinaryCharacter in compressedBinaryString)
        {
            currentCharacter.Append(compressedBinaryCharacter);
            huffmanCodes.TryGetValue(currentCharacter.ToString(), out string? originalCharacter);
            if(originalCharacter is not null)
            {
                finalBinaryString.Append(originalCharacter + " ");
                currentCharacter.Clear();
            }
        }

        return finalBinaryString.ToString().Trim();
    }
    private PriorityQueue<string, double> CalculateBinaryCharactersProbabilities(List<string> binaryCharacters)
    {
        PriorityQueue<string, double> charactersProbabilitiesPriorityQueue = new();

        //sort character and calculate probabilities
        binaryCharacters.Sort();
        binaryCharacters.Add("111111111");
        string currentCharacter = binaryCharacters.First();
        int currentCharacterCount = 0;
        for (int i = 0; i < binaryCharacters.Count; i++)
        {
            if (currentCharacter != binaryCharacters[i])
            {
                double characterProbability = Math.Round((double)currentCharacterCount / (binaryCharacters.Count - 1), 5);

                charactersProbabilitiesPriorityQueue.Enqueue($"{currentCharacter} {characterProbability}", characterProbability);
                currentCharacterCount = 0;
                currentCharacter = binaryCharacters[i];
            }

            currentCharacterCount++;
        }
        binaryCharacters.Remove("111111111");

        return charactersProbabilitiesPriorityQueue;
    }

    public string LempelZiv78Compression(string inputBinaryString)
    {
        List<string> binaryCharacters = inputBinaryString.Split(" ").ToList();
        StringBuilder compressedBinaryString = new StringBuilder();
        Dictionary<string, string> sequencesAndEncodings = new Dictionary<string, string>();
        int encoding = 256;

        StringBuilder tempBinaryCharactersBuilder = new StringBuilder();
        string potentialValue = "";

        for (int i = 0; i < binaryCharacters.Count - 1; i++)
        {
            tempBinaryCharactersBuilder.Append(binaryCharacters[i]);
            if (sequencesAndEncodings.TryGetValue(tempBinaryCharactersBuilder.ToString() + binaryCharacters[i + 1], out string? sequenceEncoding))
            {
                potentialValue = sequenceEncoding;
                continue;
            }

            if (encoding < 512)
                sequencesAndEncodings.Add(tempBinaryCharactersBuilder.ToString() + binaryCharacters[i + 1], Convert.ToString(encoding, 2));

            string finalValue = potentialValue != "" ? potentialValue : binaryCharacters[i];
            compressedBinaryString.Append(finalValue);

            potentialValue = "";
            tempBinaryCharactersBuilder.Clear();
            encoding++;
        }

        //Add the null currentCharacter in the end
        compressedBinaryString.Append("00000000");
        /*int initialByteCount = binaryCharacters.Count;
        double compressedByteCount = compressedBinaryString.Length / 8;
        double compressionRatio = (initialByteCount - compressedByteCount) / initialByteCount * 100;*/
        return compressedBinaryString.ToString();
    }
    public string LempelZiv78Decompression(string compressedBinaryString)
    {
        Dictionary<string, string> sequencesAndEncodings = new Dictionary<string, string>();
        List<string> compressedBinaryStrings = new List<string>();
        StringBuilder finalbinaryStringBuilder = new StringBuilder();
        int encoding = 256;

        int encodedCharacterSize = 0;
        while (compressedBinaryString.Count() != 0)
        {
            if (compressedBinaryString.StartsWith("0"))
                encodedCharacterSize = 8;
            else if (compressedBinaryString.StartsWith("1"))
                encodedCharacterSize = 9;

            compressedBinaryStrings.Add(compressedBinaryString.Substring(0, encodedCharacterSize));
            compressedBinaryString = compressedBinaryString.Remove(0, encodedCharacterSize);
        }

        for (int i = 0; i < compressedBinaryStrings.Count - 1; i++)
        {
            string firstValue = compressedBinaryStrings[i].StartsWith("0") ?
                  compressedBinaryStrings[i] : sequencesAndEncodings.GetValueOrDefault(compressedBinaryStrings[i])!;
            string secondValue = compressedBinaryStrings[i + 1].StartsWith("0") ?
                  compressedBinaryStrings[i + 1] : sequencesAndEncodings.GetValueOrDefault(compressedBinaryStrings[i + 1])!;

            // Handle the potential edge case where secondValue is null
            if (secondValue is null)
                secondValue = firstValue + " " + firstValue.Substring(0, 8);

            finalbinaryStringBuilder.Append(firstValue + " ");
            if (encoding < 512)
                sequencesAndEncodings.Add(Convert.ToString(encoding, 2), firstValue + " " + secondValue.Substring(0, 8));

            encoding++;
        }

        finalbinaryStringBuilder.Append("00000000");
        return finalbinaryStringBuilder.ToString();
    }

    public (string, OrderedDictionary) ArithmeticCodingCompression(string inputBinaryString)
    {
        List<string> binaryCharacters = inputBinaryString.Split(" ").ToList();

        //we find initial intervals
        OrderedDictionary binaryCharactersIntervals = CalculateBinaryCharacterInitialIntervalsAndProbabilities(binaryCharacters);
        CharProbAndInterval interval = new CharProbAndInterval(0, 0, 1);

        OrderedDictionary deepCopyOfBinaryCharacterIntervals = new OrderedDictionary();
        foreach (DictionaryEntry entry in binaryCharactersIntervals)
            deepCopyOfBinaryCharacterIntervals.Add(entry.Key, entry.Value);

        while (binaryCharacters.Count != 0)
        {
            interval = (CharProbAndInterval)binaryCharactersIntervals[binaryCharacters[0]]!;

            //if it was the last currentCharacter do not do it again
            if (binaryCharacters.Count != 1)
                //recalculate the interval
                CalculateNewIntervals(interval, binaryCharactersIntervals);

            binaryCharacters.RemoveAt(0);
        }

        decimal finalValue = (interval.LowInterval + interval.HighInterval) / 2;
        string compressedBinaryString = DecimalToBinaryFraction(finalValue, 255);
        return (compressedBinaryString, deepCopyOfBinaryCharacterIntervals);
    }
    public string ArithmeticCodingDecompression(string compressedBinaryString, OrderedDictionary binaryCharactersIntervals)
    {
        CharProbAndInterval interval = new CharProbAndInterval(0, 0, 1);
        decimal decimalPercentage = BinaryFractionToDecimal(compressedBinaryString);
        string finalString = "";

        OrderedDictionary deepCopyOfBinaryCharacterIntervals = new OrderedDictionary();
        foreach (DictionaryEntry entry in binaryCharactersIntervals)
            deepCopyOfBinaryCharacterIntervals.Add(entry.Key, entry.Value);

        while (true)
        {
            string currentCharacter = "";

            foreach (DictionaryEntry binaryCharacterIntervalAndProbability in binaryCharactersIntervals)
            {
                CharProbAndInterval currentProbAndInterval = (CharProbAndInterval)binaryCharacterIntervalAndProbability.Value!;

                if (currentProbAndInterval.LowInterval <= decimalPercentage && decimalPercentage < currentProbAndInterval.HighInterval)
                {
                    finalString += (string)binaryCharacterIntervalAndProbability.Key  == "00000000" ? "00000000" : (string)binaryCharacterIntervalAndProbability.Key + " ";
                    currentCharacter = (string)binaryCharacterIntervalAndProbability.Key;
                    break;
                }
            }

            //tilda currentCharacter or null currentCharacter
            if (currentCharacter == "00000000" || currentCharacter == "01111110") 
                break;

            interval = (CharProbAndInterval)binaryCharactersIntervals[currentCharacter]!;

            CalculateNewIntervals(interval, binaryCharactersIntervals);
        }

        return finalString;
    }
    private OrderedDictionary CalculateNewIntervals(CharProbAndInterval currentInterval, OrderedDictionary binaryCharactersIntervals)
    {
        decimal previousFoundHighInterval = currentInterval.LowInterval;

        string[] keys = new string[binaryCharactersIntervals.Count];
        binaryCharactersIntervals.Keys.CopyTo(keys, 0);

        // Iterate over the keys to update intervals
        foreach (string key in keys)
        {
            CharProbAndInterval charCurrentInterval = (CharProbAndInterval)binaryCharactersIntervals[key]!;

            charCurrentInterval.LowInterval = previousFoundHighInterval;
            charCurrentInterval.HighInterval = charCurrentInterval.LowInterval +
                (currentInterval.HighInterval - currentInterval.LowInterval) * charCurrentInterval.Probability;
            
            //Prepare the low border for the next currentCharacter
            previousFoundHighInterval = charCurrentInterval.HighInterval;
            
            //set this currentCharacter new interval
            binaryCharactersIntervals[key] = charCurrentInterval;
        }

        return binaryCharactersIntervals;
    }
    private OrderedDictionary CalculateBinaryCharacterInitialIntervalsAndProbabilities(List<string> binaryCharacters)
    {
        List<string> deepCopyOfBinaryCharacters = new List<string>();
        foreach (string binaryString in binaryCharacters)
            deepCopyOfBinaryCharacters.Add(binaryString);

        OrderedDictionary binaryCharactersProbabilities = new OrderedDictionary();
        int binaryCharactersCount = deepCopyOfBinaryCharacters.Count;
        decimal previousClosingInterval = 0; 

        while (deepCopyOfBinaryCharacters.Count != 0)
        {
            string binaryCharacter = deepCopyOfBinaryCharacters[0];
            int binaryCharacterCount = 0;
            CharProbAndInterval interval;

            foreach (string character in deepCopyOfBinaryCharacters)
            {
                if (binaryCharacter == character)
                    binaryCharacterCount++;
            }

            deepCopyOfBinaryCharacters.RemoveAll(character => binaryCharacter == character);

            interval.LowInterval = previousClosingInterval;
            interval.HighInterval = previousClosingInterval + Math.Round(binaryCharacterCount / (decimal)binaryCharactersCount, 3);
            interval.Probability = Math.Round(binaryCharacterCount / (decimal)binaryCharactersCount, 3);
            binaryCharactersProbabilities.Add(binaryCharacter, interval);
            previousClosingInterval = interval.HighInterval;
        }

        return binaryCharactersProbabilities;
    }

    private string DecimalToBinaryFraction(decimal decimalFraction, int precision)
    {
        if (decimalFraction >= 1 || decimalFraction <= 0)
            throw new ArgumentException("Input must be a fractional number between 0 and 1 (exclusive).");

        StringBuilder binaryOutput = new StringBuilder();
        for (int i = 0; i < precision; i++)
        {
            decimalFraction *= 2;

            if (decimalFraction >= 1)
            {
                binaryOutput.Append("1");
                decimalFraction -= 1;
            }
            else
                binaryOutput.Append("0");

            // stop if remaining fraction becomes zero
            if (decimalFraction == 0)
                break;
        }

        return binaryOutput.ToString();
    }

    private decimal BinaryFractionToDecimal(string binaryFraction)
    {
        decimal decimalValue = 0.0m;
        int exponent = -1;

        foreach (char bit in binaryFraction)
        {
            if (bit == '1')
                decimalValue += (decimal)Math.Pow(2, exponent);

            exponent--;
        }

        return decimalValue;
    }

    internal struct CharProbAndInterval
    {
        public decimal Probability;
        public decimal LowInterval;
        public decimal HighInterval;

        // Constructor with optional parameters
        public CharProbAndInterval(decimal probability = 0, decimal lowInterval = 0, decimal highInterval = 1)
        {
            Probability = probability;
            LowInterval = lowInterval;
            HighInterval = highInterval;
        }
    }
}
