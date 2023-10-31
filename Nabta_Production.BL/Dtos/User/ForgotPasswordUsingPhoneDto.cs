using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class ForgotPasswordUsingPhoneDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string UserName { get; set; } = string.Empty;
    }
}
