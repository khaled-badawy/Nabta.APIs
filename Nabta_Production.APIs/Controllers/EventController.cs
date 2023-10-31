using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventManager _eventManager;

        public EventController(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        [HttpGet]
        [Route("get_all")]

        public ActionResult<List<ReadEventDto>> GetAllEvents(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");
            var events = _eventManager.GetAll(pageNumber, search);

            if (events.IsNullOrEmpty()) return Ok(new {message = "لا يوجد "});
            return events;
        }

        [HttpGet]
        [Route("get")]

        public ActionResult<ReadEventDto> GetEvent(int id)
        {
            if (id < 1) return BadRequest("Id must be a positive value");
            var eventData = _eventManager.GetById(id);

            if (eventData == null) return Ok(new {message = "No event is existed with this id"});
            return eventData;
        }
    }
}
