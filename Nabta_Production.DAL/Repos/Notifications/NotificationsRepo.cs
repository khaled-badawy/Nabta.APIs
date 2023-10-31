using Microsoft.EntityFrameworkCore;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class NotificationsRepo : INotificationsRepo
    {
        private readonly ClimateConfNewContext _context;
        public NotificationsRepo(ClimateConfNewContext context)
        {
            _context = context;
        }

        public IEnumerable<ClmNotification> GetAll(int? userId, int pageNumber)
        {
            var notifications = _context.ClmNotifications
                .Where(n => n.UserId == userId || n.UserId == null && n.Active == 1)
                .Include(n => n.ParentCategory!)
                .OrderByDescending(n => n.CreateDate)
                .Skip((pageNumber - 1) * 10)
                .Take(10);

            if (notifications == null ) return Enumerable.Empty<ClmNotification>();
            return notifications;
        }
    }
}
