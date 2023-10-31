using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface INotificationsRepo
    {
        public IEnumerable<ClmNotification> GetAll(int? userId , int pageNumber);
    }
}
