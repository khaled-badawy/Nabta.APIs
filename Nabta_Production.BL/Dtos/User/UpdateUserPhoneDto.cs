using System.ComponentModel.DataAnnotations;
namespace Nabta_Production.BL
{
    public class UpdateUserPhoneDto
    {
        [Required]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "رقم الهاتف غير صحيح.")]
        public string NewUserPhone { get; set; } = string.Empty;
    }
}
