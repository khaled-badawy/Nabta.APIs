using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IAbusesReasons 
    {
       // public IEnumerable<ClmReport>? GetAll(int pageNumber);
        public ClmReport? GetById(int reportId);
    }
}
