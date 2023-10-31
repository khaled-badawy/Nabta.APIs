using Nabta_Production.DAL.Data;
namespace Nabta_Production.DAL
{
    public class EventRepo :  IEventRepo
    {
        private readonly ClimateConfNewContext _context;
        public EventRepo(ClimateConfNewContext context)
        {
            _context = context;
        }

        public IEnumerable<ClmEvent> GetAll(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return _context.ClmEvents
                        .Where(e => e.Active == 1 && e.EventDate >= DateTime.Now)
                        .OrderBy(e => e.EventDate)
                        .Skip((pageNumber - 1) * 10)
                        .Take(10);
            }
            return _context.ClmEvents
                .Where(e => e.Active == 1 && e.TitleA.Contains(search) && e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate )
                .Skip((pageNumber - 1) * 10)
                .Take(10);
        }

        public ClmEvent? GetById(int id)
        {
            var eventById = _context.ClmEvents.FirstOrDefault(e=>e.Id == id && e.Active == 1);
            if (eventById == null) return null;
            return eventById;
        }
    }
}
