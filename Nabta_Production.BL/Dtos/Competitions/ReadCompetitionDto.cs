namespace Nabta_Production.BL
{
    public class ReadCompetitionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public string? Image { get; set; } 
        public string? CompetionDate { get; set; } 
    }
}
