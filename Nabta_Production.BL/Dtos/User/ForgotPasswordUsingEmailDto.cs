
using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class ForgotPasswordUsingEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
