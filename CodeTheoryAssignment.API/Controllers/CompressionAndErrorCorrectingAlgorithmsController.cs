using CodeTheoryAssignment.API.Models;
using CodeTheoryAssignment.Library;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("ArithmeticCodingAndHamming74")]
    public IActionResult ArithmeticCodingAndHamming74([FromBody] HuffmanCompressionAndHamming74RequestModel huffmanCompressionAndCRCRequestModel)
    {
        return Ok();
    }

    [HttpPost("HuffmanAndHamming74")]
    public IActionResult HuffmanCompressionAndHamming74([FromBody] HuffmanCompressionAndHamming74RequestModel requestModel)
    {
        string compressedPaddedProtectedBinaryStringWithErrors = _encodingAlgorithmsService.ConvertBase64ToBinaryAlgorithm(requestModel.Base64String!);
        string compressedPaddedBinaryString = _errorCorrectingAlgorithmsService.HammingCode74Decoding(compressedPaddedProtectedBinaryStringWithErrors);
        string compressedBinaryString = _encodingAlgorithmsService.RemovePaddingFromHammingCodeEncoding(compressedPaddedBinaryString);
        string decompressedBinaryString = _compressionAlgorithmsService.HuffmanDecompression(requestModel.HuffmanCodes, compressedBinaryString);

        return Ok(decompressedBinaryString);
    }

    [HttpPost("LempelZiv78AndHamming74")]
    public IActionResult LempelZiv78CompressionAndHamming74([FromBody] LempelZiv78CompressionAndHamming74RequestModel lempelZiv78CompressionAndCRCRequestModel)
    {
        return Ok();
    }

    [HttpPost("ArithmeticCodingAndCRC")]
    public IActionResult ArithmeticCodingAndCRC([FromBody] HuffmanCompressionAndHamming74RequestModel huffmanCompressionAndCRCRequestModel)
    {
        return Ok();
    }

    [HttpPost("HuffmanAndCRC")]
    public IActionResult HuffmanCompressionAndCRC([FromBody] HuffmanCompressionAndHamming74RequestModel huffmanCompressionAndCRCRequestModel)
    {
        return Ok();
    }

    [HttpPost("LempelZiv78AndCRC")]
    public IActionResult LempelZiv78CompressionAndCRC([FromBody] LempelZiv78CompressionAndHamming74RequestModel lempelZiv78CompressionAndCRCRequestModel)
    {
        return Ok();
    }
}
