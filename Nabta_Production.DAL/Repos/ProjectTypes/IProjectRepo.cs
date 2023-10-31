using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IProjectRepo
    {
        IEnumerable<ClmLkpProject> GetAll();
        ClmLkpProject? GetById(int projectId);
    }
}
