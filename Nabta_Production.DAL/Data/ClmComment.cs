namespace Nabta_Production.DAL.Data;

public partial class ClmComment
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public string Comment { get; set; } = null!;

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public bool Active { get; set; }

    public virtual ClmPost Post { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
