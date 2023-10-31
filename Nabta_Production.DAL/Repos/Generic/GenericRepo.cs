using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
    {
        private readonly ClimateConfNewContext _context;
        public GenericRepo(ClimateConfNewContext context)
        {
            _context = context;
        }
        public IEnumerable<TEntity> GetAll()
        {
           return _context.Set<TEntity>();
        }

        public TEntity? GetByID(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
