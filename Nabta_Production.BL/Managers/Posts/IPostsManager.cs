namespace Nabta_Production.BL
{
    public interface IPostsManager
    {
        List<ReadPostsDto> GetAllPosts(int pageNumber , string? search);
        List<ReadPostsDto> GetMyShares(int pageNumber ,int userId);
        List<ReadNewsDto> GetNews(int pageNumber, string? search);
        List<ReadPostsDto> GetProjects(int pageNumber, string? search);
        List<ReadPostsDto> GetYourShares(int pageNumber, string? search);
        List<ReadIdeasDto> GetIdeas(int pageNumber, string? search);
        ReadPostByIdDto? GetPostById(int postID);
        Task<int> Add(AddPostDto post , int userId);
        bool LikePost(int postId);
        bool UnLikePost(int postId);
        bool DeleteUserPost(int userId, int postId);
    }
}
