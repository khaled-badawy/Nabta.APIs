using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.BL
{
    public class AttachmentManager : IAttachmentManager
    {
        private readonly IAttachmentRepo _attachmentRepo;
        private readonly IPostsRepo _postsRepo;

        public AttachmentManager(IAttachmentRepo attachmentRepo, IPostsRepo postsRepo)
        {
            _attachmentRepo = attachmentRepo;
            _postsRepo = postsRepo;
        }

        public bool Add(string fileName , int parentId, int userId)
        {
            ClmPost? post = _postsRepo.GetPostById(parentId);
            if (post == null || String.IsNullOrEmpty(fileName)) return false;

            ClmAttachment attachment = new ClmAttachment()
            {
                OwnerId = userId,
                CreateDate = DateTime.Now,
                ParentCategoryId = 1,
                ParentId = parentId,
                FileName = fileName,
                SortIndex = 0,
                Focus = 0,
                Active = 1
            };

            _attachmentRepo.Add(attachment);

            _attachmentRepo.SaveChanges();

            return true;
        }
    }
}
