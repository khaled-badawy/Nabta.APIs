namespace Nabta_Production.BL
{
    public class AddPostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int? ProjectId { get; set; }
        // public int UserId { get; set; }
       // public string? Attachment { get; set; }
    }
}
