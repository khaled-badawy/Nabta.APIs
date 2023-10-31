using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;
using Nabta_Production.DAL;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsManager _postsManager;
        private readonly IFileManager _fileManager;
        private readonly IAttachmentManager _attachmentManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(IPostsManager postsManager , IFileManager fileManager , IAttachmentManager attachmentManager, UserManager<ApplicationUser> userManager)
        {
            _postsManager = postsManager;
            _fileManager = fileManager;
            _attachmentManager = attachmentManager;
            _userManager = userManager;
        }

        #region Get All Posts

        [HttpGet]
        [Route("get_posts")]
        public ActionResult<List<ReadPostsDto>> GetPosts(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var posts = _postsManager.GetAllPosts(pageNumber, search);

            if (posts.IsNullOrEmpty()) return Ok(new {message = "لا يوجد"});
          
            return posts;
        }

        #endregion

        #region Get News

        [HttpGet]
        [Route("get_news")]
        public ActionResult<List<ReadNewsDto>> GetNews(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var news = _postsManager.GetNews(pageNumber, search);

            if (news.IsNullOrEmpty()) return Ok(new { message = "لا يوجد" });

            return news;
        }

        #endregion

        #region Get Projects

        [HttpGet]
        [Route("get_projects")]
        public ActionResult<List<ReadPostsDto>> GetProjects(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var projects = _postsManager.GetProjects(pageNumber, search);

            if (projects.IsNullOrEmpty()) return Ok(new { message = "لا يوجد" });

            return projects;
        }

        #endregion

        #region Get Ideas

        [HttpGet]
        [Route("get_ideas")]
        public ActionResult<List<ReadIdeasDto>> GetIdeas(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var ideas = _postsManager.GetIdeas(pageNumber, search);

            if (ideas.IsNullOrEmpty()) return Ok(new { message = "لا يوجد" });

            return ideas;
        }

        #endregion

        #region Get Post By Id

        [HttpGet]
        [Route("get_post")]
        public ActionResult<ReadPostByIdDto> GetPostById(int postId)
        {
            if (postId < 1) return BadRequest("Min value of post id is 1");

            var post = _postsManager.GetPostById(postId);

            if (post == null) return Ok(new { message = "لا يوجد" });

            return post;
        }

        #endregion

        #region Get Your shares posts

        [HttpGet]
        [Route("get_your_share")]
        public ActionResult<List<ReadPostsDto>> GetYourShares(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var yourShares = _postsManager.GetYourShares(pageNumber, search);

            if (yourShares.IsNullOrEmpty()) return Ok(new { message = "لا يوجد" });

            return yourShares;
        }

        #endregion

        #region Add post

        [HttpPost]
        [Authorize]
        [Route("add_post")]

        public async Task<ActionResult> AddPost([FromForm(Name = "file")] IFormFile? PostPicture , [FromForm(Name = "post")] AddPostDto postDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int postId = await _postsManager.Add(postDto, userId);

            string? testUpload = "";

            if (PostPicture != null)
            {
                string newFileName = _fileManager.RenameFile(PostPicture,$"PostAttachment_{postId}")!;

                _attachmentManager.Add(newFileName, postId, userId);

                testUpload = await _fileManager.UploadFile(PostPicture, postId, $"PostAttachment_{postId}", "post");
            }
            return Ok(new {fileName = testUpload,message = "تمت إضافة المشاركة بنجاح سوف يتم مراجعة مشاركتك" });
        }

        #endregion

        #region Like post

        [HttpPost]
        [Authorize]
        [Route("like")]

        public async Task<ActionResult> LikePost(int postId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            bool isExisted = _postsManager.LikePost(postId);
            if (!isExisted) return BadRequest(new { message = "No post existed" });
            if (user.LikedPosts == null || user.LikedPosts == string.Empty)
            {
                user.LikedPosts = $"{postId}";
            }
            else
            {
                string[] likedPosts = user.LikedPosts.Split(",");

                bool isLiked = Array.Exists(likedPosts , post=> post == postId.ToString());
                if (isLiked) return BadRequest(new {message = "Post is liked already."});
                user.LikedPosts = user.LikedPosts + $",{postId}";
            }
            await _userManager.UpdateAsync(user);
            return Ok(new { message = "post liked successfully." });
        }

        #endregion

        #region Unlike post

        [HttpDelete]
        [Authorize]
        [Route("unlike")]

        public async Task<ActionResult> UnLikePost(int postId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }
            bool isExisted = _postsManager.UnLikePost(postId);
            if (!isExisted) return BadRequest(new { message = "No post existed" });
            if (user.LikedPosts == null || user.LikedPosts == string.Empty)
            {
                return BadRequest(new { message = "Post is already unliked." });
            }
            string[] likedPosts = user.LikedPosts.Split(",");
            bool isLiked = Array.Exists(likedPosts, post => post == postId.ToString());
            if (!isLiked) return BadRequest(new { message = "Post is already unliked." });
            likedPosts = likedPosts.Where(post => post != postId.ToString()).ToArray();
            foreach (var post in likedPosts)
            {
                user.LikedPosts = string.Join(",", likedPosts);
            }
            await _userManager.UpdateAsync(user);
            return Ok(new { message = "Post unliked successfully."});
        }

        #endregion

        #region Get My shares

        [HttpGet]
        [Authorize]
        [Route("my_shares")]

        public ActionResult<List<ReadPostsDto>> GetMyShares(int pageNumber)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var userPosts = _postsManager.GetMyShares(pageNumber, userId);

            if (userPosts.IsNullOrEmpty()) return Ok(new { message = "لا يوجد مشاركات لديكم" });
            return Ok(userPosts);
        }

        #endregion

        #region Delete User Post

        [HttpDelete]
        [Authorize]
        [Route("delete_post")]

        public ActionResult DeletePost(int postId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isDeleted = _postsManager.DeleteUserPost(userId, postId);
            if (isDeleted) return Ok(new {message = "تم حذف المنشور بنجاح"});
            return Ok(new {message = "User has not published the given post id"});
        }

        #endregion
    }
}
