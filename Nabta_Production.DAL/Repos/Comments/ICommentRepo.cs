using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface ICommentRepo
    {
        ClmComment? GetByID(int id);
        void Add(ClmComment comment);
        void Update(ClmComment comment);
        void Delete(ClmComment comment);
        int SaveChanges();
    }
}
