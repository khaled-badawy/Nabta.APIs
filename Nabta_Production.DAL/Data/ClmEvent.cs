namespace Nabta_Production.DAL.Data;

public partial class ClmEvent
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string TitleA { get; set; } = null!;

    public string? TitleE { get; set; }

    public string Icon { get; set; } = null!;

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public DateTime EventDate { get; set; }
}
