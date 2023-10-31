using Nabta_Production.DAL.Data;
namespace Nabta_Production.DAL
{
    public class AbusesRepo : IAbusesRepo
    {
        private readonly ClimateConfNewContext _context;
        public AbusesRepo(ClimateConfNewContext context)
        {

            _context = context;

        }

        public void Add(ClmReportAbuse report)
        {
            _context.Add(report);
        }

        public IEnumerable<ClmLkpReportAbuseReason> GetReportsReasons()
        {
            return _context.ClmLkpReportAbuseReasons.Where(reason => reason.Active == true);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
