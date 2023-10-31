namespace Nabta_Production.DAL.Data;

public partial class ClmAttachment
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public int ParentId { get; set; }

    public int ParentCategoryId { get; set; }

    public string FileName { get; set; } = null!;

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public virtual ClmLkpAttachmentCategory ParentCategory { get; set; } = null!;
}
