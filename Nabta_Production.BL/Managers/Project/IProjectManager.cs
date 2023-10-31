namespace Nabta_Production.BL
{
    public interface IProjectManager
    {
        List<ReadProjectDto> GetAllProjects();
        ReadProjectDto? GetProjectById(int projectId);
    }
}
