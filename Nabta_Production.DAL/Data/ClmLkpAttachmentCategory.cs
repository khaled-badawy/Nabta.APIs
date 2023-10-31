namespace Nabta_Production.DAL.Data;

public partial class ClmLkpAttachmentCategory
{
    public int Id { get; set; }

    public string NameA { get; set; } = null!;

    public string? NameE { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public virtual ICollection<ClmAttachment> ClmAttachments { get; set; } = new List<ClmAttachment>();

    public virtual ICollection<ClmNotification> ClmNotifications { get; set; } = new List<ClmNotification>();
}
