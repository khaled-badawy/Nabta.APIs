using Microsoft.EntityFrameworkCore;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class PostsRepo : IPostsRepo
    {
        private readonly ClimateConfNewContext _context;

        public PostsRepo(ClimateConfNewContext context)
        {
            _context = context;
        }
        public ClmPost? GetPostById(int postId)
        {
            var postFromDB = _context.ClmPosts
                //.Include(p => p.Project)
                .Include(p => p.User)
                .Include(p => p.ClmComments).ThenInclude(p => p.User)
                .Include(p => p.Type)
                .FirstOrDefault(p => p.Id == postId && p.Active == true);
            if (postFromDB == null) return null;
            return postFromDB;
        }
        public ClmPost? GetPostForUserById(int userId, int postId)
        {
            return _context.ClmPosts
                .FirstOrDefault(p => p.Id == postId && p.UserId == userId && p.Active == true);
        }
        public async Task Add(ClmPost post)
        {
            await _context.ClmPosts.AddAsync(post);
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Delete(ClmPost post)
        {
            post.Active = false;
            SaveChanges();
        }
    }
}
