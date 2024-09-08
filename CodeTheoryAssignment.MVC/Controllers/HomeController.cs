using CodeTheoryAssignment.Library;
using CodeTheoryAssignment.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace CodeTheoryAssignment.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEncodingAlgorithmsService _encodingAlgorithmsService;
    private readonly ICompressionAlgorithmsService _compressionAlgorithmsService;
    private readonly IErrorCorrectingAlgorithmsService _errorCorrectingAlgorithmsService;
    private readonly IErrorInsertionService _errorInsertionService;
    private readonly IStatisticsService _statisticsService;

    public HomeController(ILogger<HomeController> logger, IEncodingAlgorithmsService encodingAlgorithmsService, 
        ICompressionAlgorithmsService compressionAlgorithmsService, IErrorCorrectingAlgorithmsService errorCorrectingAlgorithmsService,
        IErrorInsertionService errorInsertionService, IStatisticsService statisticsService)
    {
        _logger = logger;
        _encodingAlgorithmsService = encodingAlgorithmsService;
        _compressionAlgorithmsService = compressionAlgorithmsService;
        _errorCorrectingAlgorithmsService = errorCorrectingAlgorithmsService;
        _errorInsertionService = errorInsertionService;
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        //string text = "Hello World!";

        //Client
        //string initialBinaryString = _encodingAlgorithmsService.ConvertASCIIToBinaryAlgorithm(text);            
        //(string compressedBinaryString, Dictionary<string, string> huffmanCodes) = _compressionAlgorithmsService.HuffmanCompression(initialBinaryString);
        //(string compressedBinaryString, double compressionRate) = _compressionAlgorithmsService.LempelZiv78Compression(initialBinaryString);
        //(string compressedBinaryString, OrderedDictionary characterIntervals) = _compressionAlgorithmsService.ArithmeticCodingCompression(initialBinaryString);
        //int huffmanCodesBitCount = huffmanCodes.Sum(codeRawValue => codeRawValue.Key.Length + codeRawValue.Value.Length);

        /*double compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(),
            compressedBinaryString.Count() + (characterIntervals.Count * 8));
        */

        /*List<int> errorPositions = new List<int>();
        string compressedBinaryStringCopy = compressedBinaryString;
        Dictionary<string, int> byteAndPosition = new Dictionary<string, int>();

        //Think on how to do the retransmission
        do
        {
            string compressedCRCEncodedBinaryString = _errorCorrectingAlgorithmsService.CyclicRedundancyCodeEncoding(compressedBinaryStringCopy);
            string compressedCRCEncodedBinaryStringWithErrors = _errorInsertionService.AddErrorsBasedOnProbability(compressedCRCEncodedBinaryString, 0.1);
            (compressedBinaryStringCopy, errorPositions) = _errorCorrectingAlgorithmsService.CyclicRedundancyCodeDecoding(compressedCRCEncodedBinaryStringWithErrors);
            int index = 0;
            compressedBinaryStringCopy = "";
            while (index < compressedBinaryString.Count())
            {
                if (errorPositions.Contains(index / 8))
                {
                    compressedBinaryStringCopy += compressedBinaryString.Substring(index, 8);
                    index += 8;
                    continue;
                }

                byteAndPosition.Add(compressedBinaryString.Substring(index, 8), index);
                index += 8;
            }
        } while (errorPositions.Count > 0);*/

        //Leave it for now
        //string compressedPaddedBinaryString = _encodingAlgorithmsService.AddPaddingForHammingCodeEncoding(compressedBinaryString);
        //string compressedPaddedProtectedBinaryString = _errorCorrectingAlgorithmsService.HammingCode74Encoding(compressedPaddedBinaryString);
        //string compressedPaddedProtectedBinaryStringWithErrors = _errorInsertionService.AddOneErrorEvery7Bits(compressedPaddedProtectedBinaryString);
        //string base64String = _encodingAlgorithmsService.ConvertBinaryToBase64Algorithm(compressedPaddedProtectedBinaryStringWithErrors);

        //Server
        //compressedPaddedProtectedBinaryStringWithErrors = _encodingAlgorithmsService.ConvertBase64ToBinaryAlgorithm(base64String);
        //compressedPaddedBinaryString = _errorCorrectingAlgorithmsService.HammingCode74Decoding(compressedPaddedProtectedBinaryStringWithErrors);
        //compressedBinaryString = _encodingAlgorithmsService.RemovePaddingFromHammingCodeEncoding(compressedPaddedBinaryString);
        //string decompressedBinaryString = _compressionAlgorithmsService.HuffmanDecompression(huffmanCodes, compressedBinaryString);
        //string decompressedBinaryString = _compressionAlgorithmsService.LempelZiv78Decompression(compressedBinaryString);
        //string decompressedBinaryString = _compressionAlgorithmsService.ArithmeticCodingDecompression(compressedBinaryString, characterIntervals);
        //string finalString = _encodingAlgorithmsService.ConvertBinaryToASCIIAlgorithm(decompressedBinaryString);

        return View();
    }

    [HttpPost]
    public IActionResult Index(AlgorithmModel algorithmModel)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
