namespace Nabta_Production.BL
{
    public class ReadPostsDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        //public string? Description { get; set; }
        public int? NoOfLikes { get; set; }
        public int? NoOfViews { get; set; }
        public int? NoOfComments { get; set; }
        public int TypeId { get; set; }
        public string? TypeName { get; set; }
        public int? UserID { get; set; }
        public string? UserName { get; set; }
        public string? UserPicture { get; set; }
        public string? ShareLink { get; set; }
        public double PuplishedDate { get; set; }
        public List<string>? Attachments { get; set; }
    }
}
