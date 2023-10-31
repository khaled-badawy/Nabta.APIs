namespace Nabta_Production.BL
{
    public class ReadReportsDto
    {
        public int Id { get; set; }
        public string? CreateDate { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? SourceName { get; set; }
        public string? ShareLink { get; set; }
        public List<string>? Attachment { get; set; }
    }
}
