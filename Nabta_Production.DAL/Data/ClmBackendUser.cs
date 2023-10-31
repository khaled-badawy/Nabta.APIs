namespace Nabta_Production.DAL.Data;

public partial class ClmBackendUser
{
    public int Id { get; set; }

    public int? OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? TempPassword { get; set; }

    public string? Mobile { get; set; }

    public int? RoleId { get; set; }

    public int SortIndex { get; set; }

    public bool Active { get; set; }

    public int Focus { get; set; }
}
