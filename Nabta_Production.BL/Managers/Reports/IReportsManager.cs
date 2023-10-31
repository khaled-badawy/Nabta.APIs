namespace Nabta_Production.BL
{
    public interface IReportsManager
    {
        List<ReadReportsDto> GetAll(int pageNumber , string? search);
        ReadReportsDto? GetById(int id);
    }
}
