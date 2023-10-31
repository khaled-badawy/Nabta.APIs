using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly ClimateConfNewContext _context;

        public ProjectRepo(ClimateConfNewContext context)
        {
            _context = context;
        }
        public IEnumerable<ClmLkpProject> GetAll()
        {
            var projects = _context.ClmLkpProjects.Where(p => p.Active == true);
            if (projects.IsNullOrEmpty()) return Enumerable.Empty<ClmLkpProject>();
            return projects;
        }

        public ClmLkpProject? GetById(int projectId)
        {
            return _context.ClmLkpProjects.FirstOrDefault(p => p.Id == projectId && p.Active == true);
        }
    }
}
