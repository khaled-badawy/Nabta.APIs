using Nabta_Production.DAL;

namespace Nabta_Production.BL
{
    public class SideMenuManager : ISideMenuManager
    {
        private readonly ISideMenuContentRepo _menuRepo;

        public SideMenuManager(ISideMenuContentRepo menuRepo) 
        {
            _menuRepo = menuRepo;
        }
        public List<ReadSideMenuContentDto> GetAll()
        {
            var menuContentsFromDB = _menuRepo.GetAll().ToList();

            return menuContentsFromDB
                .Where(p => p.Active == true)
                .OrderBy(p => p.SortIndex)
                .Select(p => new ReadSideMenuContentDto
            {
                PageNameArabic = p.TitleA,
                PageNameEnglish = p.PageName!,
                Description = p.DescriptionA,
                RouterLink = p.RouterLink!,
                Icon = p.Icon!
            }).ToList();

        }

        public ReadSideMenuContentDto? GetById(int id)
        {
            var menuContentFromDB = _menuRepo.GetByID(id);

            if (menuContentFromDB == null) return null;

            return new ReadSideMenuContentDto()
            {
                PageNameArabic = menuContentFromDB.TitleA,
                PageNameEnglish = menuContentFromDB.PageName!,
                Description = menuContentFromDB.DescriptionA,
                RouterLink = menuContentFromDB.RouterLink!,
                Icon = menuContentFromDB.Icon!
            };
        }
    }
}
