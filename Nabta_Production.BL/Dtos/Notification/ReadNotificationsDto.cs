namespace Nabta_Production.BL
{
    public class ReadNotificationsDto
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? ParentCatrgoryName { get; set; }
        public string? PuplishedDate { get; set; }
        public int? MediaType { get; set; }
    }
}
