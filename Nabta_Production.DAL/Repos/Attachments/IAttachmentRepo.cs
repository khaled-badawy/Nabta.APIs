using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public interface IAttachmentRepo
    {
        void Add(ClmAttachment attachment);
        int SaveChanges();
    }
}
