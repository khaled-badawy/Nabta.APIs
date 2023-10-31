namespace Nabta_Production.DAL.Data;

public partial class ClmLkpSource
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string NameA { get; set; } = null!;

    public string? NameE { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public virtual ICollection<ClmReport> ClmReports { get; set; } = new List<ClmReport>();
}
