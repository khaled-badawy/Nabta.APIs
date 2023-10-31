using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class CommentRepo : ICommentRepo
    {
        private readonly ClimateConfNewContext _context;
        public CommentRepo(ClimateConfNewContext context)
        {
            _context = context;
        }
        public ClmComment? GetByID(int id)
        {
            return _context.ClmComments.Find(id);
        }

        public void Add(ClmComment comment)
        {
            _context.ClmComments.Add(comment);
        }

        public void Delete(ClmComment comment)
        {
            _context.ClmComments.Update(comment);
        }


        public void Update(ClmComment comment)
        {
            _context.ClmComments.Update(comment);
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

    }
}
