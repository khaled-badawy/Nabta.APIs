namespace Nabta_Production.BL
{
    public class ReadPostByIdDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? NoOfLikes { get; set; }
        public int? NoOfViews { get; set; }
        public int? NoOfComments { get; set; }
        public int TypeId { get; set; }
        public string? TypeName { get; set; }
        public int? UserID { get; set; }
        public string? UserName { get; set; }
        public string? UserPicture { get; set; }
        public int? ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectIcon { get; set; }
        public string? SourceLink { get; set; }
        public string? ShareLink { get; set; }
        public double PuplishedDate { get; set; }
        public ICollection<ReadCommentsDto>? Comments { get; set; }
        public List<string>? Attachments { get; set; }
    }
}
