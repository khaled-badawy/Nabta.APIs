using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nabta_Production.BL;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IAbusesManager _abusesManager;

        public ReportingController(IAbusesManager abusesManager)
        {
            _abusesManager = abusesManager;
        }

        [HttpGet]
        [Route("get_reasons")]

        public ActionResult<List<ReadAbusesReasonsDto>> GetAbusesReasons()
        {
            return _abusesManager.GetAbusesReasons();
        }

        [HttpPost]
        [Authorize]
        [Route("add_post_report")]

        public ActionResult ReportPost(AddReportAbusesDto reportAbuse)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int? reportId = _abusesManager.Add(reportAbuse, userId, 1);
            if (reportId.HasValue) return Ok(new { report_id = reportId , message = "تم الابلاغ بنجاح"});

            return Ok(new {message = "No post is existed with this id"});
        }

        [HttpPost]
        [Authorize]
        [Route("add_comment_report")]

        public ActionResult ReportComment(AddReportAbusesDto reportAbuse)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int? reportId = _abusesManager.Add(reportAbuse, userId, 2);
            if (reportId.HasValue) return Ok(new { report_id = reportId, message = "تم الابلاغ بنجاح" });

            return Ok(new { message = "No comment is existed with this id" });
        }
    }
}
