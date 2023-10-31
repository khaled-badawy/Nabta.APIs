namespace Nabta_Production.BL
{
    public interface ICompetitionManager
    {
        List<ReadCompetitionDto> GetAllCompetions(int pageNumber , string? search);

        int AddCompetionParticipant(AddParticipantDto participantDto , int participantId , string fileName);
    }
}
