namespace Nabta_Production.BL
{
    public interface IAbusesManager
    {
        List<ReadAbusesReasonsDto> GetAbusesReasons();
        int? Add(AddReportAbusesDto reportAbuse, int userID , int parentCategoryId);
    }
}
