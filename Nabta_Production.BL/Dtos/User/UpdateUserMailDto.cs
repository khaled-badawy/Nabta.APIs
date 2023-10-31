using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class UpdateUserMailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;
    }
}
