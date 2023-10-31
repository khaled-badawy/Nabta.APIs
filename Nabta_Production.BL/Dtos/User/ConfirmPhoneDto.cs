
using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class ConfirmPhoneDto
    {
        [Required]
        public string ActivationCode { get; set; } = string.Empty;
    }
}
