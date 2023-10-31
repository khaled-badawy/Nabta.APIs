namespace Nabta_Production.BL
{
    public class ReadMediaDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreateDate { get; set; }
        public string? ShareLink { get; set; }
        public List<string>? Attachments { get; set; }
    }
}
