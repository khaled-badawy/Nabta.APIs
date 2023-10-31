using Nabta_Production.DAL.Data;
namespace Nabta_Production.DAL
{
    public interface IInitiativesRepo
    {
        ClmInitiative? GetById(int id);
    }
}
