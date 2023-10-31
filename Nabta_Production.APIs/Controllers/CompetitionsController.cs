using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionsController : ControllerBase
    {
        private readonly ICompetitionManager _competitionManager;
        private readonly IFileManager _fileManager;

        public CompetitionsController(ICompetitionManager competitionManager , IFileManager fileManager)
        {
            _competitionManager = competitionManager;
            _fileManager = fileManager;
        }

        [HttpGet]
        [Route("get_all")]

        public ActionResult<ReadCompetitionDto> GetAllCompetitions(int pageNumber , string? search)
        {
            if (pageNumber < 1) return BadRequest(new { message = "Min value of page number is 1" });

            var competitions = _competitionManager.GetAllCompetions(pageNumber, search);
            if (competitions.IsNullOrEmpty()) return Ok(new {message = "لا يوجد مسابقات"});

            return Ok(competitions);
        }

        [HttpPost]
        [Authorize]
        [Route("add_participant")]

        public async Task<ActionResult> AddParticipant([FromForm(Name = "file")]IFormFile file ,[FromForm(Name = "participant")] AddParticipantDto participantDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var fileName = _fileManager.RenameFile(file,$"participant{userId}_Competition{participantDto.CompetitionId}")!;

            var competitionParticipantId = _competitionManager.AddCompetionParticipant(participantDto ,userId ,fileName);

            var uploadFileResult = await _fileManager.UploadFile(file, competitionParticipantId, $"participant{userId}_Competition{participantDto.CompetitionId}", "competition");

            if (uploadFileResult.IsNullOrEmpty()) return BadRequest(new {message = "Can't be uploaded"});

            return Ok(uploadFileResult);
        }
    }
}
