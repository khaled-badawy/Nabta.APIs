using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class CompetitionRepoo : ICompetitionRepo
    {
        private readonly ClimateConfNewContext _context;

        public CompetitionRepoo(ClimateConfNewContext context)
        {
            _context = context;
        }

        public IEnumerable<ClmCompetition>? GetAllCompetions(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return _context.ClmCompetitions.Where(x => x.Active == 1)
                        .OrderByDescending(x => x.CreateDate)
                        .Skip((pageNumber - 1) * 10)
                        .Take(10);
            }

            return _context.ClmCompetitions.Where(x => x.Active == 1 && (x.TitleA.Contains(search) || x.DescriptionA.Contains(search)))
            .OrderByDescending(x => x.CreateDate)
            .Skip((pageNumber - 1) * 10)
            .Take(10);
        }

        public void Add(ClmCompetitionParticipant competitionParticipant)
        {
            _context.Add(competitionParticipant);
        }
        public int SaveChange()
        {
            return _context.SaveChanges();
        }
    }
}
