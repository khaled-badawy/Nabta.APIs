namespace Nabta_Production.BL
{
    public interface INotificationsManager
    {
        List<ReadNotificationsDto> GetAll(int? userId, int pageNumber);
    }
}
