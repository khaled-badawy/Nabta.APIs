using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.BL
{
    public class AbusesManager : IAbusesManager
    {
        private readonly IAbusesRepo _abusesRepo;
        private readonly ICommentRepo _commentRepo;
        private readonly IPostsRepo _postRepo;

        public AbusesManager(IAbusesRepo abusesRepo , ICommentRepo commentRepo , IPostsRepo postRepo)
        {
            _abusesRepo = abusesRepo;
            _commentRepo = commentRepo;
            _postRepo = postRepo;
        }

        public int? Add(AddReportAbusesDto reportAbuse, int userID ,int parentCategoryId)
        {
            if (parentCategoryId == 1)
            {
                ClmPost? post = _postRepo.GetPostById(reportAbuse.ParentID);
                if (post is null) return null;
            }
            if (parentCategoryId == 2)
            {
                ClmComment? comment = _commentRepo.GetByID(reportAbuse.ParentID);
                if (comment is null) return null;
            }

            ClmReportAbuse reportToAdd = new ClmReportAbuse()
            {
                OwnerId = userID,
                UserId = userID,
                CreateDate = DateTime.Now,
                Reason = reportAbuse.Reason,
                ReasonId = reportAbuse.ReasonID,
                ParentId = reportAbuse.ParentID,
                ParentCategoryId = parentCategoryId,
                Active = 1,
                Focus = 0,
                SortIndex = 0,
            };

            _abusesRepo.Add(reportToAdd);
            _abusesRepo.SaveChanges();

            return reportToAdd.Id;
        }

        public List<ReadAbusesReasonsDto> GetAbusesReasons()
        {
            return _abusesRepo.GetReportsReasons()
                .Select(x => new ReadAbusesReasonsDto
                {
                    Id = x.Id,
                    Reason = x.NameA
                }).ToList();
        }
    }
}
