namespace CodeTheoryAssignment.Library
{
    public interface IStatisticsService
    {
        double CalculateCompressionRate(int uncompressedBitCount, int compressedBitCount);
    }
}