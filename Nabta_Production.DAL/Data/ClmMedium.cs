namespace Nabta_Production.DAL.Data;

public partial class ClmMedium
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string TitleA { get; set; } = null!;

    public string? TitleE { get; set; }

    public string? DescriptionA { get; set; }

    public string? DescriptionE { get; set; }

    public int MediaTypeId { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public bool Active { get; set; }

    public virtual ClmLkpMediaType MediaType { get; set; } = null!;
}
