namespace Nabta_Production.BL
{
    public class ReadNewsDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UserPicture { get; set; }
        public string? ShareLink { get; set; }
        public int? NoOfLikes { get; set; }
        public int? NoOfViews { get; set; }
        public int? NoOfComments { get; set; }
        public double PuplishedDate { get; set; }
        public List<string>? Attachments { get; set; }
    }
}
