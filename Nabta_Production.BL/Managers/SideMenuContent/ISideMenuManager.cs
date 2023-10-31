namespace Nabta_Production.BL
{
    public interface ISideMenuManager
    {
        List<ReadSideMenuContentDto> GetAll();
        ReadSideMenuContentDto? GetById(int id);
    }
}
