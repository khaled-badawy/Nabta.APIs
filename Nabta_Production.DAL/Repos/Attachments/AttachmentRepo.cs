using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class AttachmentRepo : IAttachmentRepo
    {
        private readonly ClimateConfNewContext _context;

        public AttachmentRepo(ClimateConfNewContext context)
        {
            _context = context;
        }
        public void Add(ClmAttachment attachment)
        {
            _context.ClmAttachments.Add(attachment);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
