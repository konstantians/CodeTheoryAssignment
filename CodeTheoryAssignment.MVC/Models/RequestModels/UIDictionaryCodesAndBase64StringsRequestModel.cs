using System.ComponentModel.DataAnnotations;

namespace CodeTheoryAssignment.MVC.Models.RequestModels;

public class UIDictionaryCodesAndBase64StringsRequestModel : UILempelZiv78CompressionAndCrcRequestModel
{
    [Required]
    public Dictionary<string, string> DictionaryCodes { get; set; } = new Dictionary<string, string>();

}
