using Microsoft.AspNetCore.Mvc;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SideMenuController : ControllerBase
    {
        private readonly ISideMenuManager _menuManager;

        public SideMenuController(ISideMenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        [HttpGet]
        [Route("get_all")]

        public ActionResult<List<ReadSideMenuContentDto>> GetAllMenuContent() 
        {
            return _menuManager.GetAll();
        }
    }
}
