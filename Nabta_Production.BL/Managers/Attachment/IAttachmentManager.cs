namespace Nabta_Production.BL
{
    public interface IAttachmentManager
    {
        bool Add(string fileName , int parentId , int userId);
    }
}
