using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class InitiativesRepo : IInitiativesRepo
    {
        private readonly ClimateConfNewContext _context;

        public InitiativesRepo(ClimateConfNewContext context)
        {
            _context = context;
        }

        public ClmInitiative? GetById(int id)
        {
            var initiativeFromDB = _context.ClmInitiatives
                .FirstOrDefault(i => i.Id == id && i.Active == 1);

            if (initiativeFromDB == null) return null;
            return initiativeFromDB;
        }
    }
}
