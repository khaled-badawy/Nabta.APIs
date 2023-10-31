using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class SideMenuContentRepo : GenericRepo<ClmStaticContent> , ISideMenuContentRepo
    {
       public SideMenuContentRepo(ClimateConfNewContext context) : base(context)
        {
        }
    }
}
