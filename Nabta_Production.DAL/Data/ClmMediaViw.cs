namespace Nabta_Production.DAL.Data;

public partial class ClmMediaViw
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public string TitleA { get; set; } = null!;

    public string? DescriptionA { get; set; }

    public int MediaTypeId { get; set; }

    public string? MediaTypeNameA { get; set; }

    public string? MediaTypeNameE { get; set; }

    public bool Active { get; set; }
}
