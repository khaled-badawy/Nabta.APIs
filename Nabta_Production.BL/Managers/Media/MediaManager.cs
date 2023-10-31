using Microsoft.Extensions.Configuration;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Globalization;

namespace Nabta_Production.BL
{
    public class MediaManager : IMediaManager
    {
        private readonly IMediaRepo _mediaRepo;
        private readonly ClimateConfNewContext _context;
        private readonly IConfiguration _config;
        private string url { get; }
        private string urlShareLink { get; }
        public MediaManager(IMediaRepo mediaRepo, ClimateConfNewContext context, IConfiguration config)
        {
            _mediaRepo = mediaRepo;
            _context = context;
            _config = config;
            url = $"{_config.GetSection("ServerDownloadPath").Value!}/media/attachments";
            urlShareLink = $"{_config.GetSection("ShareLink").Value!}/media/details";
        }

        public List<ReadMediaDto>? GetMedia(int typeId, int pageNumber , string? search)
        {
            var mediasFromDB = _mediaRepo.GetMedia(typeId, pageNumber, search)?.ToList();
            if (mediasFromDB == null) return null;

            if (typeId == 2)
            {
                return mediasFromDB
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 2 && a.Active == 1),
                media => media.Id,
                attachment => attachment.ParentId,
                (media, attachment) => new
                {
                    Media = media,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                })
                .Select(m => new ReadMediaDto
                {
                    Id = m.Media.Id,
                    Title = m.Media.TitleA.Trim(),
                    Attachments = m.Attachment.Select(a => $"{a.Trim()}").ToList(),
                    CreateDate = m.Media.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                    ShareLink = $"{urlShareLink}/{m.Media.Id}"
                }).ToList();
            }

            return mediasFromDB
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 2 && a.Active == 1),
                media => media.Id,
                attachment => attachment.ParentId,
                (media, attachment) => new
                {
                    Media = media,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                })
                .Select(m => new ReadMediaDto
                {
                    Id = m.Media.Id,
                    Title = m.Media.TitleA,
                    Attachments = m.Attachment.Select(a => $"{url}/{m.Media.Id}/{a}").ToList(),
                    CreateDate = m.Media.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                    ShareLink = $"{urlShareLink}/{m.Media.Id}"
                }).ToList();
        }

        public ReadMediaDto? GetMediaById(int mediaId)
        {
            var isExisted = _mediaRepo.GetMediaById(mediaId);
            if (isExisted == null)  return null;
            
            var mediaFromDB = _context.ClmMedia
                .GroupJoin(_context.ClmAttachments
                .Where(a => a.ParentCategoryId == 2 && a.Active == 1),
                media => media.Id,
                attachment => attachment.ParentId,
                (media, attachment) => new
                {
                    Media = media,
                    Attachment = attachment.Select(a => a.FileName).ToList(),
                })
                .FirstOrDefault(m => m.Media.Active == true && m.Media.Id == mediaId)!;

            if (mediaFromDB.Media.MediaTypeId == 2)
            {
                return new ReadMediaDto
                {
                    Id = mediaFromDB.Media.Id,
                    Title = mediaFromDB.Media.TitleA.Trim(),
                    Description = mediaFromDB.Media.DescriptionA,
                    Attachments = mediaFromDB.Attachment.Select(a => $"{a.Trim()}").ToList(),
                    CreateDate = mediaFromDB.Media.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                    ShareLink = $"{urlShareLink}/{mediaFromDB.Media.Id}"
                };
            }

            return new ReadMediaDto
            {
                Id = mediaFromDB.Media.Id,
                Title = mediaFromDB.Media.TitleA,
                Description = mediaFromDB.Media.DescriptionA,
                Attachments = mediaFromDB.Attachment.Select(a => $"{url}/{mediaFromDB.Media.Id}/{a}").ToList(),
                CreateDate = mediaFromDB.Media.CreateDate.ToString("dd MMMM yyyy", new CultureInfo("ar-AE")),
                ShareLink = $"{urlShareLink}/{mediaFromDB.Media.Id}"
            };
        }
    }
}
