using Microsoft.AspNetCore.Mvc;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateUsersIDsController : ControllerBase
    {
        private readonly ClimateConfNewContext _context;

        public UpdateUsersIDsController(ClimateConfNewContext context)
        {
            _context = context;
        }

        // Apply these endpoints after break mobile user relation with these tables

        #region Update User Post IDs

        [HttpPost]
        [Route("update_posts")]

        public ActionResult UpdatePostsWithNewUsersIds()
        {
            var oldUsersIds = _context.Users.Where(u=>u.OldId != null).Select(u => new
            {
                oldId = u.OldId,
                newId = u.Id
            }).ToList();

            foreach (var user in oldUsersIds)
            {
                var postsUserId = _context.ClmPosts.Where(p => p.UserId == user.oldId);
                foreach (var post in postsUserId)
                {
                    post.UserId = user.newId;
                }
            }

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Update User Notifications IDs

        [HttpPost]
        [Route("update_notifications")]

        public ActionResult UpdateNotificationsWithNewUsersIds() 
        {
            var oldUsersIds = _context.Users.Where(u => u.OldId != null).Select(u => new
            {
                oldId = u.OldId,
                newId = u.Id
            }).ToList();

            foreach (var user in oldUsersIds)
            {
                var postsUserId = _context.ClmNotifications.Where(p => p.UserId == user.oldId);
                foreach (var post in postsUserId)
                {
                    post.UserId = user.newId;
                }
            }

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region Update User Comments IDs

        [HttpPost]
        [Route("update_comments")]

        public ActionResult UpdateCommentsWithNewUsersIds()
        {
            var oldUsersIds = _context.Users.Where(u => u.OldId != null).Select(u => new
            {
                oldId = u.OldId,
                newId = u.Id
            }).ToList();

            foreach (var user in oldUsersIds)
            {
                var postsUserId = _context.ClmComments.Where(p => p.UserId == user.oldId);
                foreach (var post in postsUserId)
                {
                    post.UserId = user.newId;
                }
            }

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region Update User Report Abuses IDs
        [HttpPost]
        [Route("update_abuses")]

        public ActionResult UpdateAbusessWithNewUsersIds()
        {
            var oldUsersIds = _context.Users.Where(u => u.OldId != null).Select(u => new
            {
                oldId = u.OldId,
                newId = u.Id
            }).ToList();

            foreach (var user in oldUsersIds)
            {
                var postsUserId = _context.ClmReportAbuses.Where(p => p.UserId == user.oldId);
                foreach (var post in postsUserId)
                {
                    post.UserId = user.newId;
                }
            }

            _context.SaveChanges();

            return Ok();
        }
        #endregion

    }
}
