
using System.ComponentModel.DataAnnotations;
namespace Nabta_Production.BL
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword" ,ErrorMessage ="New password and confirm new password are not the same.")]
        public string Re_NewPassword { get; set; } = string.Empty;
    }
}
