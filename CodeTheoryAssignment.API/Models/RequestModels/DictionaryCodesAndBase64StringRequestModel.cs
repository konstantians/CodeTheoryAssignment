using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.API.Models.RequestModels;

public class DictionaryCodesAndBase64StringRequestModel : LempelZiv78CompressionAndHamming74RequestModel
{
    [Required]
    public Dictionary<string, string> DictionaryCodes { get; set; } = new Dictionary<string, string>();
}

