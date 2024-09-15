namespace CodeTheoryAssignment.Library;

public interface IErrorInsertionService
{
    (int, string) AddErrorsBasedOnProbability(string binarySequence, double probability);
    string AddOneErrorEvery7Bits(string binarySequence);
}