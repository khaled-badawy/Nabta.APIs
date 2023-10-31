namespace Nabta_Production.DAL
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity? GetByID(int id);
        int SaveChanges();

    }
}
