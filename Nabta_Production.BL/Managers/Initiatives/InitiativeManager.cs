using Microsoft.Extensions.Configuration;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class InitiativeManager : IInitiativeManager
    {
        private readonly ClimateConfNewContext _context;
        private readonly IInitiativesRepo _intiativesRepo;
        private readonly IConfiguration _config;
        private string url { get; }
        private string urlShareLink { get;}

        public InitiativeManager(ClimateConfNewContext context , IInitiativesRepo initiativesRepo , IConfiguration config)
        {
            _context = context;
            _intiativesRepo = initiativesRepo;
            _config = config;
            url = $"{_config.GetSection("ServerDownloadPath").Value!}/initiatives";
            urlShareLink = $"{_config.GetSection("ShareLink").Value!}/initiatives/details";
        }
        public List<ReadInitiativeDto> GetAll(int pageNumber , int typeId, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return _context.ClmInitiatives
                     .Where(r => r.Active == 1 && r.TypeId == typeId)
                        .OrderByDescending(r => r.CreateDate)
                        .GroupJoin(_context.ClmAttachments
                        .Where(a => a.ParentCategoryId == 4 && a.Active == 1),
                            initiative => initiative.Id,
                            attachment => attachment.ParentId,
                            (initiative, attachment) => new
                            {
                                Initiative = initiative,
                                Attachment = attachment.Select(a => a.FileName).ToList(),
                            })
                        .Skip((pageNumber - 1) * 10)
                        .Take(10)
                        .Select(i => new ReadInitiativeDto
                        {
                            Id = i.Initiative.Id,
                            Title = i.Initiative.TitleA,
                            Description = i.Initiative.DescriptionA,
                            Image = $"{url}/images/{i.Initiative.Id}/{i.Initiative.Image}",
                            CreateDate = i.Initiative.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                            ShareLink = $"{urlShareLink}/{i.Initiative.Id}",
                            Attachment = i.Attachment.Select(a => $"{url}/attachments/{i.Initiative.Id}/{a}").ToList()
                        }).ToList();
            }
            return _context.ClmInitiatives
                   .Where(i => i.Active == 1 && i.TypeId == typeId && (i.TitleA.Contains(search!) || i.DescriptionA!.Contains(search!)))
                      .OrderByDescending(r => r.CreateDate)
                      .GroupJoin(_context.ClmAttachments
                      .Where(a => a.ParentCategoryId == 4 && a.Active == 1),
                          initiative => initiative.Id,
                          attachment => attachment.ParentId,
                          (initiative, attachment) => new
                          {
                              Initiative = initiative,
                              Attachment = attachment.Select(a => a.FileName).ToList(),
                          })
                      .Skip((pageNumber - 1) * 10)
                      .Take(10)
                      .Select(i => new ReadInitiativeDto
                      {
                          Id = i.Initiative.Id,
                          Title = i.Initiative.TitleA,
                          Description = i.Initiative.DescriptionA,
                          Image = $"{url}/images/{i.Initiative.Id}/{i.Initiative.Image}",
                          CreateDate = i.Initiative.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                          ShareLink = $"{urlShareLink}/{i.Initiative.Id}",
                          Attachment = i.Attachment.Select(a => $"{url}/attachments/{i.Initiative.Id}/{a}").ToList()
                      }).ToList();
        }

        public ReadInitiativeDto? GetById(int id)
        {
            var isExisted = _intiativesRepo.GetById(id);
            if (isExisted == null) return null;

            var initiativeFromDB = _context.ClmInitiatives
                .OrderByDescending(r => r.CreateDate)
                        .GroupJoin(_context.ClmAttachments
                        .Where(a => a.ParentCategoryId == 4 && a.Active == 1),
                            initiative => initiative.Id,
                            attachment => attachment.ParentId,
                            (initiative, attachment) => new
                            {
                                Initiative = initiative,
                                Attachment = attachment.Select(a => a.FileName).ToList(),
                            })
                        .FirstOrDefault(r => r.Initiative.Id == id && r.Initiative.Active == 1)!;
            return new ReadInitiativeDto
            {
                Id = initiativeFromDB.Initiative.Id,
                Title = initiativeFromDB.Initiative.TitleA,
                Description = initiativeFromDB.Initiative.DescriptionA,
                CreateDate = initiativeFromDB.Initiative.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                Image = $"{url}/images/{initiativeFromDB.Initiative.Id}/{initiativeFromDB.Initiative.Image}",
                ShareLink = $"{urlShareLink}/{initiativeFromDB.Initiative.Id}",
                Attachment = initiativeFromDB.Attachment.Select(a => $"{url}/attachments/{initiativeFromDB.Initiative.Id}/{a}").ToList()
            };
        }
    }
}
