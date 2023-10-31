namespace Nabta_Production.BL
{
    public interface IMediaManager
    {
        List<ReadMediaDto>? GetMedia(int typeId, int pageNumber , string? search);
        ReadMediaDto? GetMediaById(int mediaId);
    }
}
