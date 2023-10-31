using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class CompetitionManager : ICompetitionManager
    {
        private readonly ICompetitionRepo _competionRepo;
        private readonly IConfiguration _config;
        private string url { get; }

        public CompetitionManager(ICompetitionRepo competionRepo , IConfiguration config)
        {
            _competionRepo = competionRepo;
            _config = config;
            url = $"{_config.GetSection("ServerDownloadPath").Value!}/competitions/images";
        }

        public List<ReadCompetitionDto> GetAllCompetions(int pageNumber, string? search)
        {
            IEnumerable<ClmCompetition>? competionsFromDB = _competionRepo.GetAllCompetions(pageNumber, search);

            if (competionsFromDB.IsNullOrEmpty()) return new List<ReadCompetitionDto>();

            return competionsFromDB!.Select(x => new ReadCompetitionDto
            {
                Id = x.Id,
                Title = x.TitleA,
                Description = x.DescriptionA,
                CompetionDate = x.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                Image = x.Image == null ? null : $"{url}/{x.Id}/{x.Image}"
            }).ToList();
        }

        public int AddCompetionParticipant(AddParticipantDto participantDto , int participantId , string fileName)
        {
            var participantToAdd = new ClmCompetitionParticipant
            { 
                ParticipantId = participantId ,
                FileName = fileName ,
                CompetitionId = participantDto.CompetitionId,
                CreateDate = DateTime.Now,
                SortIndex = 0,
                Focus = 0,
                Active = 1
            };

            _competionRepo.Add(participantToAdd);
            _competionRepo.SaveChange();

            return participantToAdd.Id;

        }
    }
}
