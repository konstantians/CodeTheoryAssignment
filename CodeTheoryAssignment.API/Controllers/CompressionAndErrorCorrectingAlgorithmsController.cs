using CodeTheoryAssignment.API.Models.RequestModels;
using CodeTheoryAssignment.API.Models.ResponseModels;
using CodeTheoryAssignment.Library;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CodeTheoryAssignment.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CompressionAndErrorCorrectingAlgorithmsController : ControllerBase
{
    private readonly IEncodingAlgorithmsService _encodingAlgorithmsService;
    private readonly ICompressionAlgorithmsService _compressionAlgorithmsService;
    private readonly IErrorCorrectingAlgorithmsService _errorCorrectingAlgorithmsService;
    private readonly IErrorInsertionService _errorInsertionService;
    private readonly IStatisticsService _statisticsService;
    private static Dictionary<int, string> returnedPositionsAndDecodedCharacters = new Dictionary<int, string>(); //hac field that will help with simulating CRC. Possible issues with concurrency...

    public CompressionAndErrorCorrectingAlgorithmsController(IEncodingAlgorithmsService encodingAlgorithmsService, 
        ICompressionAlgorithmsService compressionAlgorithmsService, IErrorCorrectingAlgorithmsService errorCorrectingAlgorithmsService, 
        IErrorInsertionService errorInsertionService, IStatisticsService statisticsService)
    {
        _encodingAlgorithmsService = encodingAlgorithmsService;
        _compressionAlgorithmsService = compressionAlgorithmsService;
        _errorCorrectingAlgorithmsService = errorCorrectingAlgorithmsService;
        _errorInsertionService = errorInsertionService;
        _statisticsService = statisticsService;
    }

    /*[HttpPost("ArithmeticCodingAndHamming74")]
    public IActionResult ArithmeticCodingAndHamming74([FromBody] DictionaryCodesAndBase64StringRequestModel huffmanCompressionAndCRCRequestModel)
    {
        return Ok();
    }*/

    [HttpPost("ShannonFanoAndHamming74")]
    public IActionResult ShannonFanoCompressionAndHamming74([FromBody] DictionaryCodesAndBase64StringRequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithHamming74(requestModel);
    }


    [HttpPost("HuffmanAndHamming74")]
    public IActionResult HuffmanCompressionAndHamming74([FromBody] DictionaryCodesAndBase64StringRequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithHamming74(requestModel);
    }

    [HttpPost("LempelZiv78AndHamming74")]
    public IActionResult LempelZiv78CompressionAndHamming74([FromBody] LempelZiv78CompressionAndHamming74RequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithHamming74(new DictionaryCodesAndBase64StringRequestModel() { Base64String = requestModel.Base64String, DictionaryCodes = null!}, true);
    }

    /*[HttpPost("ArithmeticCodingAndCRC")]
    public IActionResult ArithmeticCodingAndCRC([FromBody] DictionaryCodesAndBase64StringRequestModel huffmanCompressionAndCRCRequestModel)
    {
        return Ok();
    }*/

    [HttpPost("ShannonFanoAndCRC")]
    public IActionResult ShannonFanoCompressionAndCRC([FromBody] DictionaryCodesAndBase64StringsRequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithCrc(requestModel);
    }

    [HttpPost("HuffmanAndCRC")]
    public IActionResult HuffmanCompressionAndCRC([FromBody] DictionaryCodesAndBase64StringsRequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithCrc(requestModel);
    }

    [HttpPost("LempelZiv78AndCRC")]
    public IActionResult LempelZiv78CompressionAndCRC([FromBody] LempelZiv78CompressionAndCrcRequestModel requestModel)
    {
        return CommonCompressionAlgorithmPartWhenCombinedWithCrc(new DictionaryCodesAndBase64StringsRequestModel() { Base64PositionsAndStrings = requestModel.Base64PositionsAndStrings, DictionaryCodes = null!}, true);
    }

    private IActionResult CommonCompressionAlgorithmPartWhenCombinedWithHamming74(DictionaryCodesAndBase64StringRequestModel requestModel, bool usedForLempelZiv78 = false)
    {
        string compressedPaddedProtectedBinaryStringWithErrors = _encodingAlgorithmsService.ConvertBase64ToBinaryAlgorithm(requestModel.Base64String!);
        (string compressedPaddedBinaryString, int detectedErrors) = _errorCorrectingAlgorithmsService.HammingCode74Decoding(compressedPaddedProtectedBinaryStringWithErrors);
        string compressedBinaryString = _encodingAlgorithmsService.RemovePaddingFromHammingCodeEncoding(compressedPaddedBinaryString);
        string decompressedBinaryString = usedForLempelZiv78 ? _compressionAlgorithmsService.LempelZiv78Decompression(compressedBinaryString) :
            _compressionAlgorithmsService.HuffmanOrShannonFanoDecompression(requestModel.DictionaryCodes, compressedBinaryString);
        string asciiString = _encodingAlgorithmsService.ConvertBinaryToASCIIAlgorithm(decompressedBinaryString);

        var responseModel = new ServerGeneralResponseModel(compressedPaddedProtectedBinaryStringWithErrors, compressedPaddedBinaryString, compressedBinaryString, decompressedBinaryString, asciiString, detectedErrors);
        return Ok(responseModel);
    }

    private IActionResult CommonCompressionAlgorithmPartWhenCombinedWithCrc(DictionaryCodesAndBase64StringsRequestModel requestModel, bool usedForLempelZiv78 = false)
    {
        var compressedPaddedProtectedBinaryStringsWithErrors = new Dictionary<int, string>();
        foreach (KeyValuePair<int, string> positionAndString in requestModel.Base64PositionsAndStrings)
            compressedPaddedProtectedBinaryStringsWithErrors.Add(positionAndString.Key, _encodingAlgorithmsService.ConvertBase64ToBinaryAlgorithm(positionAndString.Value));

        //how do you keep the previous parts? One way is with static, but then it would make the REST API not be restful, the other is to send everything, but then 
        //that everything will not be realistic since there will be indicators that those were corrected and they should not contain errors... So I think the static is 
        //fine, but I will have to explain this in the assignment. That is a hack for sure...
        List<int> errorPositions = new List<int>();
        foreach (var positionAndCrcCharacterWithPotentialError in compressedPaddedProtectedBinaryStringsWithErrors)
        {
            (string crcDecodedString, List<int> errorPositionsInOneCharacter) = _errorCorrectingAlgorithmsService.CyclicRedundancyCodeDecoding(positionAndCrcCharacterWithPotentialError.Value);
            //at least 1 error was found for the character
            if (errorPositionsInOneCharacter.Count != 0)
                errorPositions.Add(positionAndCrcCharacterWithPotentialError.Key);
            //no problems were found
            else
                returnedPositionsAndDecodedCharacters.Add(positionAndCrcCharacterWithPotentialError.Key, crcDecodedString);
        }

        if (errorPositions.Count > 0)
            return Ok(new ServerGeneralResponseModel(null, null, null, null, null, errorPositions));

        List<string> tempList = returnedPositionsAndDecodedCharacters.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
        StringBuilder compressedPaddedBinaryStringBuilder = new StringBuilder();
        foreach (string temp in tempList)
            compressedPaddedBinaryStringBuilder.Append(temp);

        string compressedPaddedBinaryString = compressedPaddedBinaryStringBuilder.ToString();
        string compressedBinaryString = _encodingAlgorithmsService.RemovePaddingFromBinarySequence(compressedPaddedBinaryString);
        string decompressedBinaryString = usedForLempelZiv78 ? _compressionAlgorithmsService.LempelZiv78Decompression(compressedBinaryString) :
            _compressionAlgorithmsService.HuffmanOrShannonFanoDecompression(requestModel.DictionaryCodes, compressedBinaryString);
        string asciiString = _encodingAlgorithmsService.ConvertBinaryToASCIIAlgorithm(decompressedBinaryString);

        var responseModel = new ServerGeneralResponseModel(compressedPaddedBinaryString, compressedPaddedBinaryString, compressedBinaryString, decompressedBinaryString, asciiString, 0);
        returnedPositionsAndDecodedCharacters.Clear(); //reset the static field
        return Ok(responseModel);
    }
}
