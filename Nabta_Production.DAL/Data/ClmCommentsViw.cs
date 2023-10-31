namespace Nabta_Production.DAL.Data;

public partial class ClmCommentsViw
{
    public int Id { get; set; }

    public DateTime Createdate { get; set; }

    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? ProfilePicture { get; set; }

    public int PostId { get; set; }

    public string Comment { get; set; } = null!;

    public bool CommentActive { get; set; }

    public bool? UserActive { get; set; }
}
