using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsManager _reportsManager;

        public ReportsController(IReportsManager reportsManager)
        {
            _reportsManager = reportsManager;
        }

        [HttpGet]
        [Route("get_all")]
        public ActionResult<List<ReadReportsDto>> GetReports(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var reports = _reportsManager.GetAll(pageNumber , search);

            if (reports.IsNullOrEmpty()) return Ok(new {message = "لا يوجد تقارير"});
            return reports;
        }

        [HttpGet]
        [Route("get_report")]
        public ActionResult<ReadReportsDto?> GetReportById(int reportId)
        {
            if (reportId < 1) return BadRequest("Id must be a positive value");

            var report = _reportsManager.GetById(reportId);

            if (report == null) return Ok(new {message = "No report is existed with this id"});
            return report;
        }
    }
}
