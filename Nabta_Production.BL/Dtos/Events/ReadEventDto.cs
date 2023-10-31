namespace Nabta_Production.BL
{
    public class ReadEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? EventDate { get; set; }
    }
}
