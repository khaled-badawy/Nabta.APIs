namespace Nabta_Production.BL
{
    public interface IInitiativeManager 
    {
        List<ReadInitiativeDto> GetAll(int pageNumber , int typeId, string? search);
        ReadInitiativeDto? GetById(int id);
    }
}
