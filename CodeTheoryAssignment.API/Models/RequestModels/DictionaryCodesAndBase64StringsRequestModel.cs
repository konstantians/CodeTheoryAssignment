using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.API.Models.RequestModels;

public class DictionaryCodesAndBase64StringsRequestModel : LempelZiv78CompressionAndCrcRequestModel
{
    [Required]
    public Dictionary<string, string> DictionaryCodes { get; set; } = new Dictionary<string, string>();
}
