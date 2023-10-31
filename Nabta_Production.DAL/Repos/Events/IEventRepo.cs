using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IEventRepo 
    {
        public IEnumerable<ClmEvent> GetAll(int pageNumber , string? search);
        public ClmEvent? GetById(int id);
    }
}
