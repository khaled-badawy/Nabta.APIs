namespace Nabta_Production.BL
{
    public class AuthModel
    {
        public string? Message { get; set; } 
        public string? UserFullName { get; set; } 
        public string? Email { get; set; } 
        public string? Token { get; set; } 
       // public string? ProfilePicture { get; set; } 
        public bool IsAuthenticated { get; set; }
        //public List<string> Roles { get; set; } = new List<string>();
       // public DateTime? ExpiresOn { get; set; }

    }
}
