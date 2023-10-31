using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class ResetPasswordDto
    {
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string Re_NewPassword { get; set; } = string.Empty;

        //public string UserId { get; set; } = string.Empty;
        public string EmailTokenString { get; set; } = string.Empty;

    }
}
