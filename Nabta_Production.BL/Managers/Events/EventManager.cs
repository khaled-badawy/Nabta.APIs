using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class EventManager : IEventManager
    {
        private readonly IEventRepo _eventRepo;
        private readonly IConfiguration _config;
        private string url { get;}

        public EventManager(IEventRepo eventRepo , IConfiguration config)
        {
            _eventRepo = eventRepo;
            _config = config;
            url = _config.GetSection("ServerDownloadPath").Value!;
        }
        public List<ReadEventDto> GetAll(int pageNumber, string? search)
        {
            var eventsFromDB = _eventRepo.GetAll(pageNumber , search);

            if (eventsFromDB.IsNullOrEmpty()) return new List<ReadEventDto>();

            return eventsFromDB.Select(e=> new ReadEventDto
            {
                Id = e.Id,
                EventDate = e.EventDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                Icon = $"{url}/events/attachments/{e.Id}/{e.Icon}",
                Title = e.TitleA
            }).ToList();

        }

        public ReadEventDto? GetById(int id)
        {
            var eventFromDB = _eventRepo.GetById(id);

            if (eventFromDB == null) return null;

            return new ReadEventDto
            {
                Id = id,
                EventDate = eventFromDB.EventDate.ToString("dd MMMM yyyy",new CultureInfo("ar-AE")),
                Icon = $"{url}/events/attachments/{eventFromDB.Id}/{eventFromDB.Icon}",
                Title = eventFromDB.TitleA
            };
        }
    }
}
