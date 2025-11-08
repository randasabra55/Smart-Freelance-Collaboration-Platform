using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Common.Responses;


namespace Smart_Freelance_Service.Abstracts
{
    public interface IProjectService
    {
        // public Task<Result<ProjectDto>> CreateProject(Project project);
        public Task<Result> EditProject(long id, Project project);
        public Task<Result> DeleteProject(long id);
        public Task<Result> GetProjectById(long id);
        //public Task<Result> GetProjects();

    }
}
