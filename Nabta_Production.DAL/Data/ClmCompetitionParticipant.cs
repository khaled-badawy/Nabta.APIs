using System.ComponentModel.DataAnnotations.Schema;

namespace Nabta_Production.DAL.Data;

public partial class ClmCompetitionParticipant
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string FileName { get; set; } = null!;

    public int ParticipantId { get; set; }

    public int CompetitionId { get; set; }

    public int? IsApproved { get; set; }

    public int? ReviewerId { get; set; }

    public int SortIndex { get; set; }

    public int Focus { get; set; }

    public int Active { get; set; }

    [ForeignKey("CompetitionId")]
    public virtual ClmCompetition Competition { get; set; } = null!;

    [ForeignKey("ParticipantId")]
    public virtual ApplicationUser Participant { get; set; } = null!;

}
