using CodeTheoryAssignment.Library;
using CodeTheoryAssignment.MVC.Models;
using CodeTheoryAssignment.MVC.Models.RequestModels;
using CodeTheoryAssignment.MVC.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace CodeTheoryAssignment.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEncodingAlgorithmsService _encodingAlgorithmsService;
    private readonly ICompressionAlgorithmsService _compressionAlgorithmsService;
    private readonly IErrorCorrectingAlgorithmsService _errorCorrectingAlgorithmsService;
    private readonly IErrorInsertionService _errorInsertionService;
    private readonly IStatisticsService _statisticsService;
    private readonly HttpClient httpClient;

    public HomeController(ILogger<HomeController> logger, IEncodingAlgorithmsService encodingAlgorithmsService,
        ICompressionAlgorithmsService compressionAlgorithmsService, IErrorCorrectingAlgorithmsService errorCorrectingAlgorithmsService,
        IErrorInsertionService errorInsertionService, IStatisticsService statisticsService, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _encodingAlgorithmsService = encodingAlgorithmsService;
        _compressionAlgorithmsService = compressionAlgorithmsService;
        _errorCorrectingAlgorithmsService = errorCorrectingAlgorithmsService;
        _errorInsertionService = errorInsertionService;
        _statisticsService = statisticsService;
        httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ClientSideAsciiRepresentation"] = TempData["ClientSideAsciiRepresentation"]?.ToString() ?? "";
        ViewData["ClientSideBinaryRepresentation"] = TempData["ClientSideBinaryRepresentation"]?.ToString() ?? "";
        ViewData["ClientSideCompressedBinaryRepresentation"] = TempData["ClientSideCompressedBinaryRepresentation"]?.ToString() ?? "";
        ViewData["ClientSideCompressedPaddedBinaryRepresentation"] = TempData["ClientSideCompressedPaddedBinaryRepresentation"]?.ToString() ?? "";
        ViewData["ClientSideCompressedBinaryRepresentationWithErrorCorrecton"] = TempData["ClientSideCompressedBinaryRepresentationWithErrorCorrecton"]?.ToString() ?? "";
        ViewData["ClientSideBinaryWithErrors"] = TempData["ClientSideBinaryWithErrors"]?.ToString() ?? "";
        ViewData["ClientSideBase64Representation"] = TempData["ClientSideBase64Representation"]?.ToString() ?? "";
        ViewData["ClientSideBase64InAscii"] = TempData["ClientSideBase64InAscii"]?.ToString() ?? "";

        ViewData["ServerSideBase64Representation"] = TempData["ServerSideBase64Representation"]?.ToString() ?? "";
        ViewData["ServerSideCompressedBinaryRepresentationWithErrorCorrection"] = TempData["ServerSideCompressedBinaryRepresentationWithErrorCorrection"]?.ToString() ?? "";
        ViewData["ServerSideCompressedBinaryRepresentation"] = TempData["ServerSideCompressedBinaryRepresentation"]?.ToString() ?? "";
        ViewData["ServerSideBinaryRepresentation"] = TempData["ServerSideBinaryRepresentation"]?.ToString() ?? "";
        ViewData["ServerSideAsciiRepresentation"] = TempData["ServerSideAsciiRepresentation"]?.ToString() ?? "";

        ViewData["CompressionRate"] = TempData["CompressionRate"]?.ToString() ?? "0";
        ViewData["Entropy"] = TempData["Entropy"]?.ToString() ?? "0";
        ViewData["ErrorsAdded"] = TempData["ErrorsAdded"]?.ToString() ?? "0";
        ViewData["ErrorsDetected"] = TempData["ErrorsDetected"]?.ToString() ?? "0";
        ViewData["RetransmissionCount"] = TempData["RetransmissionCount"]?.ToString() ?? "0";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(AlgorithmModel algorithmModel)
    {
        if(algorithmModel.UserInput is null)
            return RedirectToAction("Index", "Home");
        
        string trimmedUserInput = algorithmModel.UserInput!.Trim();
        double entropy = _statisticsService.CalculateEntropy(trimmedUserInput);
        string initialBinaryString = _encodingAlgorithmsService.ConvertASCIIToBinaryAlgorithm(trimmedUserInput);

        //TODO I think LempelZiv78 crashes if it finds something that does not make sense(an error), make it not crash and allow for errors
        if (algorithmModel.CompressionAlgorithm == "ShannonFano" && algorithmModel.ErrorCorrectingAlgorithm == "Hamming74")
            await CompressionAlgorithmAndHamming74SubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);
        if (algorithmModel.CompressionAlgorithm == "Huffman" && algorithmModel.ErrorCorrectingAlgorithm == "Hamming74")
            await CompressionAlgorithmAndHamming74SubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);
        else if (algorithmModel.CompressionAlgorithm == "LempelZiv78" && algorithmModel.ErrorCorrectingAlgorithm == "Hamming74")
            await CompressionAlgorithmAndHamming74SubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);

        else if (algorithmModel.CompressionAlgorithm == "ShannonFano" && algorithmModel.ErrorCorrectingAlgorithm == "CRC")
            await CompressionAlgorithmAndCrcSubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);
        else if (algorithmModel.CompressionAlgorithm == "Huffman" && algorithmModel.ErrorCorrectingAlgorithm == "CRC")
            await CompressionAlgorithmAndCrcSubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);
        else if (algorithmModel.CompressionAlgorithm == "LempelZiv78" && algorithmModel.ErrorCorrectingAlgorithm == "CRC")
            await CompressionAlgorithmAndCrcSubAction(trimmedUserInput, initialBinaryString, entropy, algorithmModel);

        return RedirectToAction("Index", "Home"); //should not get to this point ever
    }

    private async Task<IActionResult> CompressionAlgorithmAndHamming74SubAction(string trimmedUserInput, string initialBinaryString, double entropy, AlgorithmModel algorithmModel)
    {
        double compressionRate = 0;
        string compressedBinaryString = "";
        Dictionary<string, string> dictionaryCodes = new Dictionary<string, string>();
        if(algorithmModel.CompressionAlgorithm == "Huffman")
        {
            (compressedBinaryString, dictionaryCodes) = _compressionAlgorithmsService.HuffmanCompression(initialBinaryString);
            int huffmanCodesBitCount = dictionaryCodes.Sum(codeRawValue => codeRawValue.Key.Length + codeRawValue.Value.Length);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(),
                compressedBinaryString.Count() + huffmanCodesBitCount);
        }
        else if(algorithmModel.CompressionAlgorithm == "ShannonFano")
        {
            (compressedBinaryString, dictionaryCodes) = _compressionAlgorithmsService.ShannonFanoCompression(initialBinaryString);
            int huffmanCodesBitCount = dictionaryCodes.Sum(codeRawValue => codeRawValue.Key.Length + codeRawValue.Value.Length);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(),
                compressedBinaryString.Count() + huffmanCodesBitCount);
        }
        else if(algorithmModel.CompressionAlgorithm == "LempelZiv78")
        {
            compressedBinaryString = _compressionAlgorithmsService.LempelZiv78Compression(initialBinaryString);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(), compressedBinaryString.Count());
        }

        string compressedPaddedBinaryString = _encodingAlgorithmsService.AddPaddingForHammingCodeEncoding(compressedBinaryString);
        string compressedPaddedProtectedBinaryString = _errorCorrectingAlgorithmsService.HammingCode74Encoding(compressedPaddedBinaryString);

        string compressedPaddedProtectedBinaryStringWithErrors = "";
        int errorCount = 0;
        if (algorithmModel.BitErrorChance == 0)
            compressedPaddedProtectedBinaryStringWithErrors = _errorInsertionService.AddOneErrorEvery7Bits(compressedPaddedProtectedBinaryString);
        else
            (errorCount, compressedPaddedProtectedBinaryStringWithErrors) = _errorInsertionService.AddErrorsBasedOnProbability(compressedPaddedProtectedBinaryString, (double)algorithmModel.BitErrorChance / 100);

        string base64String = _encodingAlgorithmsService.ConvertBinaryToBase64Algorithm(compressedPaddedProtectedBinaryStringWithErrors);

        HttpResponseMessage result = new HttpResponseMessage();
        if (algorithmModel.CompressionAlgorithm == "Huffman")
        {
            var requestModel = new UIDictionaryCodesAndBase64StringRequestModel() { DictionaryCodes = dictionaryCodes, Base64String = base64String };
            result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/HuffmanAndHamming74", requestModel);
        }
        else if (algorithmModel.CompressionAlgorithm == "ShannonFano")
        {
            var requestModel = new UIDictionaryCodesAndBase64StringRequestModel() { DictionaryCodes = dictionaryCodes, Base64String = base64String };
            result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/ShannonFanoAndHamming74", requestModel);
        }
        else if (algorithmModel.CompressionAlgorithm == "LempelZiv78")
        {
            var requestModel = new UILempelZiv78CompressionAndHamming74RequestModel() { Base64String = base64String };
            result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/LempelZiv78AndHamming74", requestModel);
        }

        return await PrepareViewDataAndRedirectToIndex(trimmedUserInput, initialBinaryString, compressedBinaryString, compressedPaddedBinaryString, compressedPaddedProtectedBinaryString,
            compressedPaddedProtectedBinaryStringWithErrors, base64String, compressionRate, entropy, errorCount, 0, result);
    }

    private async Task<IActionResult> CompressionAlgorithmAndCrcSubAction(string trimmedUserInput, string initialBinaryString, double entropy, AlgorithmModel algorithmModel)
    {
        double bitErrorProbability = algorithmModel.BitErrorChance == 0 ? 0.1 : (double)algorithmModel.BitErrorChance / 100;
        double compressionRate = 0;
        string compressedBinaryString = "";
        Dictionary<string, string> dictionaryCodes = new Dictionary<string, string>();
        if (algorithmModel.CompressionAlgorithm == "Huffman")
        {
            (compressedBinaryString, dictionaryCodes) = _compressionAlgorithmsService.ShannonFanoCompression(initialBinaryString);
            int huffmanCodesBitCount = dictionaryCodes.Sum(codeRawValue => codeRawValue.Key.Length + codeRawValue.Value.Length);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(),
                compressedBinaryString.Count() + huffmanCodesBitCount);
        }
        else if(algorithmModel.CompressionAlgorithm == "ShannonFano")
        {
            (compressedBinaryString, dictionaryCodes) = _compressionAlgorithmsService.ShannonFanoCompression(initialBinaryString);
            int huffmanCodesBitCount = dictionaryCodes.Sum(codeRawValue => codeRawValue.Key.Length + codeRawValue.Value.Length);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(),
                compressedBinaryString.Count() + huffmanCodesBitCount);
        }
        else if(algorithmModel.CompressionAlgorithm == "LempelZiv78")
        {
            compressedBinaryString = _compressionAlgorithmsService.LempelZiv78Compression(initialBinaryString);
            compressionRate = _statisticsService.CalculateCompressionRate(initialBinaryString.Replace(" ", "").Count(), compressedBinaryString.Count());
        }

        string compressedPaddedBinaryString = _encodingAlgorithmsService.AddPaddingToBinarySequence(compressedBinaryString);

        var positionsAndCRCEncodedCharacters = new Dictionary<int, string>();
        var positionsAndCRCEncodedCharactersWithErrors = new Dictionary<int, string>();
        int finalErrorCount = 0;

        for (int i = 0; i < compressedPaddedBinaryString.Length; i += 8)
        {
            string crcEncodedString = _errorCorrectingAlgorithmsService.CyclicRedundancyCodeEncoding(compressedPaddedBinaryString.Substring(i, 8));
            positionsAndCRCEncodedCharacters.Add(i / 8, crcEncodedString);
            (finalErrorCount, string crcEncodedStringWithPotentialErrors) = _errorInsertionService.AddErrorsBasedOnProbability(crcEncodedString, bitErrorProbability);
            positionsAndCRCEncodedCharactersWithErrors.Add(i / 8, crcEncodedStringWithPotentialErrors);
        }

        int errorsDetectedCount = 0;
        int retransmissionCount = 0;
        var returnedValues = new ServerGeneralResponseModel();
        var result = new HttpResponseMessage();
        while (true)
        {
            //convert the remaining crc strings that to base64 before making the request
            Dictionary<int, string> positionsAndBase64Strings = new Dictionary<int, string>();
            foreach (var positionAndCrcEncodedCharacterWithPotentialErrors in positionsAndCRCEncodedCharactersWithErrors)
                positionsAndBase64Strings.Add(positionAndCrcEncodedCharacterWithPotentialErrors.Key, _encodingAlgorithmsService.ConvertBinaryToBase64Algorithm(positionAndCrcEncodedCharacterWithPotentialErrors.Value));

            if (algorithmModel.CompressionAlgorithm == "Huffman")
            {
                var requestModel = new UIDictionaryCodesAndBase64StringsRequestModel() { DictionaryCodes = dictionaryCodes, Base64PositionsAndStrings = positionsAndBase64Strings };
                result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/HuffmanAndCrc", requestModel);
            }
            else if (algorithmModel.CompressionAlgorithm == "ShannonFano")
            {
                var requestModel = new UIDictionaryCodesAndBase64StringsRequestModel() { DictionaryCodes = dictionaryCodes, Base64PositionsAndStrings = positionsAndBase64Strings };
                result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/ShannonFanoAndCrc", requestModel);
            }
            else if (algorithmModel.CompressionAlgorithm == "LempelZiv78")
            {
                var requestModel = new UILempelZiv78CompressionAndCrcRequestModel() { Base64PositionsAndStrings = positionsAndBase64Strings };
                result = await httpClient.PostAsJsonAsync("CompressionAndErrorCorrectingAlgorithms/LempelZiv78AndCrc", requestModel);
            }

            returnedValues = await result.Content.ReadFromJsonAsync<ServerGeneralResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (returnedValues!.ErrorPositions.Count == 0)
                break;

            errorsDetectedCount += returnedValues.ErrorPositions.Count;

            List<int> positionsWithNoError = new List<int>();
            foreach (KeyValuePair<int, string> positionAndCharacter in positionsAndCRCEncodedCharactersWithErrors)
            {
                if (!returnedValues.ErrorPositions.Contains(positionAndCharacter.Key))
                    positionsWithNoError.Add(positionAndCharacter.Key);
            }

            foreach (int positionWithNoError in positionsWithNoError)
                positionsAndCRCEncodedCharactersWithErrors.Remove(positionWithNoError);

            //then those crc encoded that had errors make sure that they are retransmitted once again
            while (returnedValues.ErrorPositions.Count != 0)
            {
                //remove the faulty crc encoded character
                positionsAndCRCEncodedCharactersWithErrors.Remove(returnedValues.ErrorPositions.FirstOrDefault());
                //find the original crc encoded character before errors occured
                positionsAndCRCEncodedCharacters.TryGetValue(returnedValues.ErrorPositions.FirstOrDefault(), out string? crcEncodedStringThatNeedsToBeRetransmitted);
                //retransmit it with potential errors again being added
                (int errorCount, string crcEncodedStringWithPotentialErrors) = _errorInsertionService.AddErrorsBasedOnProbability(crcEncodedStringThatNeedsToBeRetransmitted!, bitErrorProbability);
                finalErrorCount += errorCount;
                positionsAndCRCEncodedCharactersWithErrors.Add(returnedValues.ErrorPositions.FirstOrDefault(), crcEncodedStringWithPotentialErrors);
                returnedValues.ErrorPositions.RemoveAt(0);
            }

            retransmissionCount++;
        }

        if (returnedValues is null)
            return RedirectToAction("index", "home");

        returnedValues.ErrorsDetected = errorsDetectedCount; //assign in the end the total count of the detected errors throughout all the retransmissions
        List<string> tempList = positionsAndCRCEncodedCharacters.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
        StringBuilder compressedPaddedProtectedBinaryStringBuilder = new StringBuilder();
        foreach (string temp in tempList)
            compressedPaddedProtectedBinaryStringBuilder.Append(temp);

        return await PrepareViewDataAndRedirectToIndex(trimmedUserInput, initialBinaryString, compressedBinaryString, compressedPaddedBinaryString, compressedPaddedProtectedBinaryStringBuilder.ToString(),
            " ", " ", compressionRate, entropy, finalErrorCount, retransmissionCount, result, returnedValues);
    }

    public async Task<IActionResult> PrepareViewDataAndRedirectToIndex(string trimmedUserInput, string initialBinaryString, string compressedBinaryString, string compressedPaddedBinaryString,
        string compressedPaddedProtectedBinaryString, string compressedPaddedProtectedBinaryStringWithErrors, string base64String, double compressionRate, double entropy, int errorCount, int retransmissionCount,
        HttpResponseMessage result, ServerGeneralResponseModel? serverGeneralResponseModel = null)
    {
        var returnedValues = serverGeneralResponseModel ?? await result.Content.ReadFromJsonAsync<ServerGeneralResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        string formattedCompressedBinaryString = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(compressedBinaryString);
        string formattedCompressedPaddedBinaryString = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(compressedPaddedBinaryString);
        string formattedCompressedProtectedBinaryString = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(compressedPaddedProtectedBinaryString);
        string formattedCompressedProtectedBinaryStringWithErrors = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(compressedPaddedProtectedBinaryStringWithErrors);
        string formattedBase64String = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(base64String);
        string base64InAscii = formattedBase64String == " " ? "Not used for given configuration" : _encodingAlgorithmsService.ConvertBinaryToASCIIAlgorithm(formattedBase64String);

        TempData["ClientSideAsciiRepresentation"] = trimmedUserInput;
        TempData["ClientSideBinaryRepresentation"] = initialBinaryString;
        TempData["ClientSideCompressedBinaryRepresentation"] = formattedCompressedBinaryString;
        TempData["ClientSideCompressedPaddedBinaryRepresentation"] = formattedCompressedPaddedBinaryString;
        TempData["ClientSideCompressedBinaryRepresentationWithErrorCorrecton"] = formattedCompressedProtectedBinaryString;
        TempData["ClientSideBinaryWithErrors"] = formattedCompressedProtectedBinaryStringWithErrors == " " ? "Not used for given configuration" : formattedCompressedProtectedBinaryStringWithErrors;
        TempData["ClientSideBase64Representation"] = formattedBase64String == " " ? "Not used for given configuration" : formattedBase64String;
        TempData["ClientSideBase64InAscii"] = base64InAscii;

        TempData["ServerSideBase64Representation"] = formattedBase64String == " " ? "Not used for given configuration" : formattedBase64String;
        TempData["ServerSideCompressedBinaryRepresentationWithErrorCorrection"] = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(returnedValues!.CompressedBinaryRepresentationWithErrorCorrection!);
        TempData["ServerSideCompressedBinaryRepresentation"] = _encodingAlgorithmsService.SplitBinarySequenceIntoBytes(returnedValues!.CompressedBinaryRepresentation!);
        TempData["ServerSideBinaryRepresentation"] = returnedValues!.BinaryRepresentation!;
        TempData["ServerSideAsciiRepresentation"] = returnedValues!.AsciiRepresentation!;

        TempData["CompressionRate"] = compressionRate.ToString();
        TempData["Entropy"] = entropy.ToString();
        TempData["ErrorsAdded"] = errorCount.ToString();
        TempData["ErrorsDetected"] = returnedValues.ErrorsDetected!;
        TempData["RetransmissionCount"] = retransmissionCount.ToString();

        return RedirectToAction("Index", "Home");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
