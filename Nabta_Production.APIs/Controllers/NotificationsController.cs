using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsManager _notificationsManager;
        public NotificationsController(INotificationsManager notificationsManager)
        {
            _notificationsManager = notificationsManager;
        }

        [HttpGet]
        [Authorize]
        [Route("get_notifications")]

        public ActionResult<List<ReadNotificationsDto>?> GetNotifications(int pageNumber)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            int? userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = _notificationsManager.GetAll(userId, pageNumber);
            if (notifications.IsNullOrEmpty()) return Ok(new {message = "لا يوجد اشعارات"});
            return Ok(notifications);
        }
    }
}
