namespace Nabta_Production.DAL.Data;

public partial class ClmPost
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string TitleA { get; set; } = null!;

    public string? TitleE { get; set; }

    public string DescriptionA { get; set; } = null!;

    public string? DescriptionE { get; set; }

    public int TypeId { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public bool Active { get; set; }

    public int? NoOfLikes { get; set; }

    public int? NoOfComments { get; set; }

    public int? NoOfViews { get; set; }

    public string? SourceLink { get; set; }

    public virtual ICollection<ClmComment> ClmComments { get; set; } = new List<ClmComment>();

    public virtual ClmLkpContentType Type { get; set; } = null!;
    public virtual ClmLkpProject Project { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
}
