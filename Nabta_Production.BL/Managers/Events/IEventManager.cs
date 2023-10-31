namespace Nabta_Production.BL
{
    public interface IEventManager
    {
        List<ReadEventDto> GetAll(int pageNumber ,string? search);
        ReadEventDto? GetById(int id);
    }
}
