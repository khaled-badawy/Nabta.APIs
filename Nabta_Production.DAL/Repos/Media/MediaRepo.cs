using Nabta_Production.DAL.Data;
namespace Nabta_Production.DAL
{
    public class MediaRepo : IMediaRepo
    {
        private readonly ClimateConfNewContext _context;
        public MediaRepo(ClimateConfNewContext context)
        {

            _context = context;

        }
        public IEnumerable<ClmMedium>? GetMedia(int typeId , int pageNumber, string? search)
        {
            if (String.IsNullOrEmpty(search))
            {
            var medias = _context.ClmMedia
                .Where(m => m.Active == true && m.MediaTypeId == typeId)
                .OrderByDescending(m => m.CreateDate)
                .Skip((pageNumber - 1) *10)
                .Take(10);
                if (medias is null) return null;
                return medias;
            }
            else
            {
               var medias = _context.ClmMedia
                    .Where(m => m.Active == true && m.MediaTypeId == typeId && (m.TitleA.Contains(search) || m.DescriptionA.Contains(search)))
                    .OrderByDescending(m => m.CreateDate)
                    .Skip((pageNumber - 1) * 10)
                    .Take(10);
                if (medias is null) return null;
                return medias;
            }
        }

        public ClmMedium? GetMediaById(int mediaId)
        {
            ClmMedium? media = _context.ClmMedia.Find(mediaId);
            return media;
        }
    }
}
