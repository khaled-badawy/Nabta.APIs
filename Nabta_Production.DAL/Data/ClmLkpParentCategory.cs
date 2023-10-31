namespace Nabta_Production.DAL.Data;

public partial class ClmLkpParentCategory
{
    public int Id { get; set; }

    public string NameA { get; set; } = null!;

    public string? NameE { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    public virtual ICollection<ClmReportAbuse> ClmReportAbuses { get; set; } = new List<ClmReportAbuse>();
}
