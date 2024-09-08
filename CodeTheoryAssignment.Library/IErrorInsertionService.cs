namespace CodeTheoryAssignment.Library
{
    public interface IErrorInsertionService
    {
        string AddErrorsBasedOnProbability(string binarySequence, double probability);
        string AddOneErrorEvery7Bits(string binarySequence);
    }
}