namespace CodeTheoryAssignment.Library;

public class StatisticsService : IStatisticsService
{
    public double CalculateCompressionRate(int uncompressedBitCount, int compressedBitCount)
    {
        double uncompressedByteCount = uncompressedBitCount / 8;
        double compressedByteCount = compressedBitCount / 8;
        return (uncompressedByteCount - compressedByteCount) / uncompressedByteCount * 100;
    }

    //TODO add entropy calculation

    //TODO maybe add an error calculation here. Think about this
}
