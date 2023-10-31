namespace Nabta_Production.DAL.Data;

public partial class ClmUser
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? RegistrationIp { get; set; }

    public string? LastActivityIp { get; set; }

    public string? Password { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public string? ActivationCode { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public int? LoginCount { get; set; }

    public bool? IsLocked { get; set; }

    public bool? IsVerified { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public bool Active { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? ProfilePicture { get; set; }

    public bool? IsDeleted { get; set; }

    public string? LikedPosts { get; set; }
}
