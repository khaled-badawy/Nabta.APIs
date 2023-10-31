using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.BL
{
    public class CommentManager : ICommentManager
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IPostsRepo _postRepo;

        public CommentManager(ICommentRepo commentRepo, IPostsRepo postRepo)
        {
            _commentRepo = commentRepo;
            _postRepo = postRepo;
        }
        public int? Add(AddCommentDto comment, int userId)
        {
            ClmPost post = _postRepo.GetPostById(comment.PostId)!;

            if (post == null) return null;

            ClmComment commentToAdd = new ClmComment()
            {
                OwnerId = userId,
                CreateDate = DateTime.Now,
                UserId = userId,
                PostId = comment.PostId,
                Comment = comment.Comment,
                SortIndex = 0,
                Focus = 0,
                Active = true,
            };
            _commentRepo.Add(commentToAdd);


            post.NoOfComments += 1;

            _commentRepo.SaveChanges();
            return commentToAdd.Id;
        }

        public bool Delete(int commentId , int userId)
        {
            ClmComment? commentToDelete = _commentRepo.GetByID(commentId);
            if (commentToDelete is null || commentToDelete.UserId != userId) return false;

            commentToDelete.Active = false;
            _commentRepo.Delete(commentToDelete);
            _commentRepo.SaveChanges();
            return true;
        }
    }
}
