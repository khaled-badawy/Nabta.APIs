namespace Nabta_Production.DAL.Data;

public partial class ClmReportAbuse
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public int UserId { get; set; }

    public int ParentId { get; set; }

    public string? Reason { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public int? ParentCategoryId { get; set; }

    public int ReasonId { get; set; }

    public virtual ClmLkpParentCategory? ParentCategory { get; set; }

    public virtual ClmLkpReportAbuseReason ReasonNavigation { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
