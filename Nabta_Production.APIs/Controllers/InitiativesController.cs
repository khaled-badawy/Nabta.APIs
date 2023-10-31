using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitiativesController : ControllerBase
    {
        private readonly IInitiativeManager _initiativeManager;

        public InitiativesController(IInitiativeManager initiativeManager)
        {
            _initiativeManager = initiativeManager;
        }

        [HttpGet]
        [Route("get_initiatives")]

        public ActionResult<List<ReadInitiativeDto>> GetInitiatives(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var initiatives = _initiativeManager.GetAll(pageNumber,1,search);
            if (initiatives.IsNullOrEmpty()) return Ok(new { message = "لا يوجد مبادرات" });
            return initiatives;
        }

        [HttpGet]
        [Route("get_projects")]

        public ActionResult<List<ReadInitiativeDto>> GetProjects(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var projects = _initiativeManager.GetAll(pageNumber, 2, search);
            if (projects.IsNullOrEmpty()) return Ok(new { message = "لا يوجد مشاريع" });
            return projects;
        }

        [HttpGet]
        [Route("get")]

        public ActionResult<List<ReadInitiativeDto>> GetInitiativeById(int initiativeId)
        {
            var initiative = _initiativeManager.GetById(initiativeId);
            if (initiative is null) return Ok(new { message = "No initiative is existed with this id" });
            return Ok(initiative);
        }
    }
}
