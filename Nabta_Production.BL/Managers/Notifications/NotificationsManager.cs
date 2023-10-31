using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class NotificationsManager : INotificationsManager
    {
        private readonly INotificationsRepo _notificationsRepo;
        private readonly ClimateConfNewContext _context;

        public NotificationsManager(INotificationsRepo notificationsRepo , ClimateConfNewContext context)
        {
            _notificationsRepo = notificationsRepo;
            _context = context;
        }

        public List<ReadNotificationsDto> GetAll(int? userId , int pageNumber)
        {
            var notificationsFromDB = _notificationsRepo.GetAll(userId,pageNumber).ToList();
            if (notificationsFromDB.IsNullOrEmpty()) return new List<ReadNotificationsDto>();
            
            return notificationsFromDB.Select(n => new ReadNotificationsDto
            {
                Id = n.Id,
                Title = n.TitleA,
                Body = n.BodyA,
                PuplishedDate = n.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                ParentId = n.ParentId,
                ParentCatrgoryName = n.ParentCategory?.NameA,
                MediaType = n.ParentCategoryId != 2 ? null :  _context.ClmMedia.FirstOrDefault(m => m.Id == n.ParentId)!.MediaTypeId,
            }).ToList();
        }
    }
}
