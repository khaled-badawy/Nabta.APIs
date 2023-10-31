namespace Nabta_Production.BL
{
    public class TokenDto
    {
       // public int UserId { get; set; } 
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
    }
}
