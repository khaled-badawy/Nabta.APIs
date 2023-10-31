using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class UpdateUserNameDto
    {
        [Required]
        public string NewUserName { get; set;} = string.Empty;
    }
}
