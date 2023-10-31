using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IAbusesRepo
    {
        IEnumerable<ClmLkpReportAbuseReason> GetReportsReasons();
        void Add(ClmReportAbuse report);
        int SaveChanges();
    }
}
