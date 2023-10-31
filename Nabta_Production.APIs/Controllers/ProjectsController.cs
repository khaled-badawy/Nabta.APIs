using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectManager _projectManager;

        public ProjectsController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        [HttpGet]
        [Route("get_all")]

        public ActionResult<List<ReadProjectDto>> GetAll()
        {
            var projectsFromDb = _projectManager.GetAllProjects();
            if (projectsFromDb.IsNullOrEmpty()) return NoContent();
            
            return Ok(projectsFromDb);
        }

        [HttpGet]
        [Route("get_project")]

        public ActionResult<List<ReadProjectDto>> GetById(int projectId)
        {
            var projectFromDb = _projectManager.GetProjectById(projectId);
            if (projectFromDb == null) return NoContent();

            return Ok(projectFromDb);
        }
    }
}
