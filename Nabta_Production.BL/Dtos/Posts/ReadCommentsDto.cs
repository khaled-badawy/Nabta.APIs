namespace Nabta_Production.BL
{
    public class ReadCommentsDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserPicture { get; set; }
        public double CommentDate { get; set; }
    }
}
