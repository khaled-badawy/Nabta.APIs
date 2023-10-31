using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;

namespace Nabta_Production.BL
{
    public class ProjectManager : IProjectManager
    {
        private readonly IProjectRepo _projectRepo;
        private readonly IConfiguration _config;
        private string urlProjectIcon { get; }

        public ProjectManager(IProjectRepo projectRepo , IConfiguration config)
        {
            _projectRepo = projectRepo;
            _config = config;
            urlProjectIcon = $"{_config.GetSection("ServerDownloadPath").Value!}/project";
        }
        public List<ReadProjectDto> GetAllProjects()
        {
            var projectsFromDb = _projectRepo.GetAll();
            if (projectsFromDb.IsNullOrEmpty()) return new List<ReadProjectDto>();
            return projectsFromDb.Select(x => new ReadProjectDto
                        {
                            Id = x.Id,
                            Title = x.NameA,
                            Icon = $"{urlProjectIcon}/{x.Id}/{x.Icon}",
                        }).ToList();
        }

        public ReadProjectDto? GetProjectById(int projectId)
        {
            var project = _projectRepo.GetById(projectId);
            if (project is null ) return null;

            return new ReadProjectDto
            {
                Id = project.Id,
                Title = project.NameA,
                Icon =$"{urlProjectIcon}/{project.Id}/{project.Icon}"
            };
        }
    }
}
