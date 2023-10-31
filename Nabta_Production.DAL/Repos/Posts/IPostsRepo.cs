using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IPostsRepo
    {
      // IEnumerable<ClmPost>? GetAll(int pageNumber);
      //  IEnumerable<ClmPost>? GetNews(int pageNumber);
      //  IEnumerable<ClmPost>? GetProjects(int pageNumber);
      //  IEnumerable<ClmPost>? GetIdeas(int pageNumber);
      //  IEnumerable<ClmPost>? GetBySearch(int pageNumber , string search);
        ClmPost? GetPostById(int postId);
        ClmPost? GetPostForUserById(int userId,int postId);
        Task Add(ClmPost post);
        void Delete(ClmPost post);
        int SaveChanges();
    }
}
