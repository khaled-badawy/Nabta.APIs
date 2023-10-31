using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IMediaRepo
    {
        IEnumerable<ClmMedium>? GetMedia(int typeId, int pageNumber, string? search);
        ClmMedium? GetMediaById(int mediaId);
    }
}
