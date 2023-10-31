
using System.ComponentModel.DataAnnotations;
namespace Nabta_Production.BL
{
    public class DeleteUserDto
    {
        [Required]
        public string UserPassword { get; set; } = string.Empty;
    }
}
