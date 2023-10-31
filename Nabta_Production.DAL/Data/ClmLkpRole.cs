namespace Nabta_Production.DAL.Data;

public partial class ClmLkpRole
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string NameA { get; set; } = null!;

    public string? NameE { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public bool Active { get; set; }
}
