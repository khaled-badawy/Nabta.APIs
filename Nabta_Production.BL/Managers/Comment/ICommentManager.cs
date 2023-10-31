namespace Nabta_Production.BL
{
    public interface ICommentManager
    {
        int? Add(AddCommentDto comment , int userId);
       // bool Update(AddCommentDto comment);
        bool Delete(int commentId , int userID);
    }
}
