using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface ICompetitionRepo
    {
        IEnumerable<ClmCompetition>? GetAllCompetions(int pageNumber , string? search);

        void Add(ClmCompetitionParticipant competitionParticipant);

        int SaveChange();
    }
}
