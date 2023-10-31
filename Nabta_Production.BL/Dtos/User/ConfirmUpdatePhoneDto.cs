using System.ComponentModel.DataAnnotations;
namespace Nabta_Production.BL
{
    public class ConfirmUpdatePhoneDto
    {
        [Required]
        public string ActivationCode { get; set; } = string.Empty;

        [Required]
        public string NewPhoneNumber { get; set; } = string.Empty;
    }
}
