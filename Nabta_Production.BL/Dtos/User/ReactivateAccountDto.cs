using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class ReactivateAccountDto
    {
        [Required(ErrorMessage ="Phone number is required")]
        [DataType(DataType.Password)]
        public string UserName { get; set; } = string.Empty;

        public DateTime RequestDate { get; } = DateTime.Now;
    }
}
