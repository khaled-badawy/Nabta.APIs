using Microsoft.AspNetCore.Identity;
using Nabta_Production.DAL.Data;

namespace Nabta_Production.DAL
{
    public class ApplicationUser : IdentityUser<int>
    {
        public DateTime? CreateDate { get; set; }
        public string? FullName { get; set; }
        public string? ActivationCode { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool? IsForgotPassEmailConfirmed { get; set; }
        public string? RegistrationIp { get; set; }
        public string? LastActivityIp { get; set; }
        public string? ProfilePicture { get; set; }
        public int? LoginCount { get; set; }
        public int? Focus { get; set; }
        public int? Active { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? Birthdate { get; set; }
        public int? OldId { get; set; }
        public string? LikedPosts { get; set; }

        public virtual ICollection<ClmComment> ClmComments { get; set; } = new List<ClmComment>();
        public virtual ICollection<ClmNotification> ClmNotifications { get; set; } = new List<ClmNotification>();
        public virtual ICollection<ClmPost> ClmPosts { get; set; } = new List<ClmPost>();
        public virtual ICollection<ClmReportAbuse> ClmReportAbuses { get; set; } = new List<ClmReportAbuse>();
        public virtual ICollection<ClmCompetitionParticipant> ClmCompetitionParticipant { get; set; } = new List<ClmCompetitionParticipant>();
    }
}
