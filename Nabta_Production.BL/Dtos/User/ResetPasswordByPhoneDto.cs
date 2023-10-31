using System.ComponentModel.DataAnnotations;
namespace Nabta_Production.BL
{
    public class ResetPasswordByPhoneDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string Re_NewPassword { get; set; } = string.Empty;

        public string ActivationCode { get; set; } = string.Empty;
        //public string UserToken { get; set; } = string.Empty;
        public string ResetPasswordToken { get; set; } = string.Empty;
    }
}
