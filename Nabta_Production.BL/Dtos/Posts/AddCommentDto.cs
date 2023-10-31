using System.ComponentModel.DataAnnotations;

namespace Nabta_Production.BL
{
    public class AddCommentDto
    {
        [Required(ErrorMessage = "Post id is required")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "يجب ادخال تعليق")]
        public string Comment { get; set; } = string.Empty;

    }
}
