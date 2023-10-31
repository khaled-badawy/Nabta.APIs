using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class ReportsManager : IReportsManager
    {
        private readonly IAbusesReasons _reportsRepo;
        private readonly IConfiguration _config;
        public ClimateConfNewContext _context { get; }
        private string url { get; }
        private string urlShareLink { get; }

        public ReportsManager(IAbusesReasons reportsRepo, IConfiguration config , ClimateConfNewContext context)
        {
            _reportsRepo = reportsRepo;
            _config = config;
            _context = context;
            url = $"{ _config.GetSection("ServerDownloadPath").Value!}/reports/images";
            urlShareLink = $"{_config.GetSection("ShareLink").Value!}/reports/details";
        }
        public List<ReadReportsDto> GetAll(int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return _context.ClmReports
                        .Include(r => r.Source)
                        .Where(r => r.Active == 1)
                        .OrderByDescending(r => r.CreateDate)
                        .GroupJoin(_context.ClmAttachments
                        .Where(a => a.ParentCategoryId == 3 && a.Active == 1),
                            report => report.Id,
                            attachment => attachment.ParentId,
                            (report, attachment) => new
                            {
                                Report = report,
                                Attachment = attachment.Select(a => a.FileName).ToList(),
                            })
                        .Skip((pageNumber - 1) * 10)
                        .Take(10)
                        .Select(r => new ReadReportsDto
                        {
                            Id = r.Report.Id,
                            CreateDate = r.Report.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                            Title = r.Report.TitleA,
                            Description = r.Report.DescriptionA,
                            Image = $"{url}/{r.Report.Id}/{r.Report.Image}",
                            SourceName = r.Report.Source != null ? r.Report.Source.NameA : null,
                            Attachment = r.Attachment.Select(a => $"{url}/{r.Report.Id}/{a}").ToList(),
                            ShareLink = $"{urlShareLink}/{r.Report.Id}"
                        }).ToList();
            }
            return _context.ClmReports
                      .Include(r => r.Source)
                      .Where(r => r.Active == 1 && (r.TitleA.Contains(search) || r.DescriptionA!.Contains(search)))
                      .OrderByDescending(r => r.CreateDate)
                      .GroupJoin(_context.ClmAttachments
                      .Where(a => a.ParentCategoryId == 3 && a.Active == 1),
                          report => report.Id,
                          attachment => attachment.ParentId,
                          (report, attachment) => new
                          {
                              Report = report,
                              Attachment = attachment.Select(a => a.FileName).ToList(),
                          })
                      .Skip((pageNumber - 1) * 10)
                      .Take(10)
                      .Select(r => new ReadReportsDto
                      {
                          Id = r.Report.Id,
                          CreateDate = r.Report.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                          Title = r.Report.TitleA,
                          Description = r.Report.DescriptionA,
                          Image = $"{url}/{r.Report.Id}/{r.Report.Image}",
                          SourceName = r.Report.Source != null ? r.Report.Source.NameA : null,
                          Attachment = r.Attachment.Select(a => $"{url}/{r.Report.Id}/{a}").ToList(),
                          ShareLink = $"{urlShareLink}/{r.Report.Id}"
                      }).ToList();
        }

        public ReadReportsDto? GetById(int id)
        {
            var isExisted = _reportsRepo.GetById(id);
            if (isExisted == null) return null;
            var reportFromDB = _context.ClmReports
                        .Include(r => r.Source)
                        .OrderByDescending(r => r.CreateDate)
                        .GroupJoin(_context.ClmAttachments
                        .Where(a => a.ParentCategoryId == 3 && a.Active == 1),
                            report => report.Id,
                            attachment => attachment.ParentId,
                            (report, attachment) => new
                            {
                                Report = report,
                                Attachment = attachment.Select(a => a.FileName).ToList(),
                            })
                        .FirstOrDefault(r => r.Report.Id == id && r.Report.Active == 1);

            if (reportFromDB is null) return null;

            ReadReportsDto report = new ReadReportsDto
            {
                Id = reportFromDB.Report.Id,
                CreateDate = reportFromDB.Report.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                Title = reportFromDB.Report.TitleA,
                Description = reportFromDB.Report.DescriptionA,
                Image = $"{url}/{reportFromDB.Report.Id}/{reportFromDB.Report.Image}",
                SourceName = reportFromDB.Report.Source?.NameA,
                Attachment = reportFromDB.Attachment.Select(a => $"{url}/{reportFromDB.Report.Id}/{a}").ToList(),
                ShareLink = $"{urlShareLink}/{reportFromDB.Report.Id}"
            };
            return report;
        }
    }
}
