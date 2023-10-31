using Microsoft.AspNetCore.Mvc;
using Nabta_Production.BL;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaManager _mediaManager;

        public MediaController(IMediaManager mediaManager)
        {
            _mediaManager = mediaManager;
        }

        [HttpGet]
        [Route("get_infograph")]

        public ActionResult<List<ReadMediaDto>?> GetInfographs(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var infographs = _mediaManager.GetMedia(1, pageNumber , search);
            if (infographs?.Count == 0 || infographs == null) return Ok(new { message = "لا يوجد انفوجراف" });
            return Ok(infographs);
        }

        [HttpGet]
        [Route("get_video")]

        public ActionResult<List<ReadMediaDto>?> GetVideos(int pageNumber, string? search)
        {
            if (pageNumber < 1) return BadRequest("Min value of page number is 1");

            var videos = _mediaManager.GetMedia(2, pageNumber, search);
            if (videos?.Count == 0 || videos == null) return Ok(new { message = "لا يوجد فيديوهات" });
            return Ok(videos);
        }

        [HttpGet]
        [Route("get_media")]

        public ActionResult<ReadMediaDto?> GetMediaById(int mediaId)
        {
            if (mediaId < 1) return BadRequest("Min value of page number is 1");

            var media =  _mediaManager.GetMediaById(mediaId);
            if (media == null) return Ok(new { message = "لا يوجد " });
            return media;
        }
    }
}
