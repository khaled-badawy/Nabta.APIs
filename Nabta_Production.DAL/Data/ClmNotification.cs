namespace Nabta_Production.DAL.Data;

public partial class ClmNotification
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string TitleA { get; set; } = null!;

    public string? TitleE { get; set; }

    public string? BodyA { get; set; }

    public string? BodyE { get; set; }

    public int? UserId { get; set; }

    public int? ReceiverId { get; set; }

    public int? ParentId { get; set; }

    public int? ParentCategoryId { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public int? OwnerId { get; set; }

    public virtual ClmLkpAttachmentCategory? ParentCategory { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
