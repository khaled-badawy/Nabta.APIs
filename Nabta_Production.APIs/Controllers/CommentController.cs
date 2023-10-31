using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nabta_Production.BL;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentManager _commentManager;

        public CommentController(ICommentManager commentManager)
        {
            _commentManager = commentManager;
        }

        [HttpPost]
        [Authorize]
        [Route("add")]

        public ActionResult AddComment(AddCommentDto commentDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (commentDto.PostId <= 0)
            {
                return BadRequest("Post id must be positive");
            }

            int? commentId = _commentManager.Add(commentDto ,userId);
            if (commentId is not null ) return Ok(new {comment_id = commentId});
            return BadRequest(new { message = "Post is not existed with this id" });
        }

        [HttpDelete]
        [Authorize]
        [Route("delete")]

        public ActionResult DeleteComment(int commentId)
        {
            if (commentId <= 0) return BadRequest(new {message = "comment id must be positive"});

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool isDeleted = _commentManager.Delete(commentId , userId);
            if (isDeleted) return Ok(new {message = "تم حذف التعليق بنجاح"});
            return BadRequest(new {message = "different user try to delete a comment or no comment is existed with provided commentId"});
        }
    }
}
