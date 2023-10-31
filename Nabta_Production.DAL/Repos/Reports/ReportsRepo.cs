using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class ReportsRepo :  IAbusesReasons
    {
        private readonly ClimateConfNewContext _context;
        public ReportsRepo(ClimateConfNewContext context) 
        {
            _context = context;
        }

        //public IEnumerable<ClmReport>? GetAll(int pageNumber)
        //{
        //    var reports = _context.ClmReports
        //                    .Include(r => r.Source)
        //                    .Where(r => r.Active == 1)
        //                    .OrderByDescending(r => r.CreateDate)
        //                    .Skip((pageNumber - 1 ) * 10)
        //                    .Take(10);
        //    if (reports == null) return null;
        //    return reports;
        //}

        public ClmReport? GetById(int reportId)
        {
            var report = _context.ClmReports
                           .FirstOrDefault(r => r.Id == reportId);
            if (report == null) return null;
            return report;
        }
    }
}
