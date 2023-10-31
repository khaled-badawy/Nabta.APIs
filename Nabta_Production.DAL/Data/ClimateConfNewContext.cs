using Microsoft.EntityFrameworkCore;

namespace Nabta_Production.DAL.Data;

public partial class ClimateConfNewContext : NabtaContext
{
   
    public ClimateConfNewContext(DbContextOptions<ClimateConfNewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClmCompetition> ClmCompetitions { get; set; }

    public virtual DbSet<ClmCompetitionParticipant> ClmCompetitionParticipants { get; set; }

    public virtual DbSet<ClmAttachment> ClmAttachments { get; set; }

    public virtual DbSet<ClmBackendUser> ClmBackendUsers { get; set; }

    public virtual DbSet<ClmComment> ClmComments { get; set; }

    public virtual DbSet<ClmCommentsViw> ClmCommentsViws { get; set; }

    public virtual DbSet<ClmEvent> ClmEvents { get; set; }

    public virtual DbSet<ClmInitiative> ClmInitiatives { get; set; }

    public virtual DbSet<ClmLkpAttachmentCategory> ClmLkpAttachmentCategories { get; set; }

    public virtual DbSet<ClmLkpContentType> ClmLkpContentTypes { get; set; }

    public virtual DbSet<ClmLkpInitiativeType> ClmLkpInitiativeTypes { get; set; }

    public virtual DbSet<ClmLkpMediaType> ClmLkpMediaTypes { get; set; }

    public virtual DbSet<ClmLkpParentCategory> ClmLkpParentCategories { get; set; }

    public virtual DbSet<ClmLkpProject> ClmLkpProjects { get; set; }

    public virtual DbSet<ClmLkpReportAbuseReason> ClmLkpReportAbuseReasons { get; set; }

    public virtual DbSet<ClmLkpRole> ClmLkpRoles { get; set; }

    public virtual DbSet<ClmLkpSource> ClmLkpSources { get; set; }

    public virtual DbSet<ClmMediaViw> ClmMediaViws { get; set; }

    public virtual DbSet<ClmMedium> ClmMedia { get; set; }

    public virtual DbSet<ClmNotification> ClmNotifications { get; set; }

    public virtual DbSet<ClmPost> ClmPosts { get; set; }

    public virtual DbSet<ClmReport> ClmReports { get; set; }

    public virtual DbSet<ClmReportAbuse> ClmReportAbuses { get; set; }

    public virtual DbSet<ClmStaticContent> ClmStaticContents { get; set; }

    public virtual DbSet<ClmUser> ClmUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Nabta_test_server");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.UseCollation("Arabic_CI_AS");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClmCompetition>(entity =>
        {
            entity.ToTable("CLM_Competitions");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE)
                .HasMaxLength(4000)
                .HasColumnName("Description_E");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
        });

        modelBuilder.Entity<ClmCompetitionParticipant>(entity =>
        {
            entity.ToTable("CLM_CompetitionParticipants");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.ParticipantId).HasColumnName("ParticipantID");
            entity.Property(e => e.ReviewerId).HasColumnName("ReviewerID");

            entity.HasOne(d => d.Competition).WithMany(p => p.ClmCompetitionParticipants)
                .HasForeignKey(d => d.CompetitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_CompetitionParticipants_CLM_Competitions");
        });

        modelBuilder.Entity<ClmAttachment>(entity =>
        {
            entity.ToTable("CLM_Attachment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.ClmAttachments)
                .HasForeignKey(d => d.ParentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_Attachment_CLM_LKP_AttachmentCategory");
        });

        modelBuilder.Entity<ClmBackendUser>(entity =>
        {
            entity.ToTable("CLM_BackendUser");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Mobile).HasMaxLength(64);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.TempPassword).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<ClmComment>(entity =>
        {
            entity.ToTable("CLM_Comments");

            entity.HasIndex(e => e.UserId, "IX_CLM_Comments_UserID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Comment).HasMaxLength(4000);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.ClmComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_Comments_CLM_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.ClmComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ClmCommentsViw>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CLM_Comments_VIW");

            entity.Property(e => e.Comment).HasMaxLength(4000);
            entity.Property(e => e.Createdate).HasColumnType("datetime");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.ProfilePicture).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<ClmEvent>(entity =>
        {
            entity.ToTable("CLM_Events");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
        });

        modelBuilder.Entity<ClmInitiative>(entity =>
        {
            entity.ToTable("CLM_Initiatives");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE)
                .HasMaxLength(4000)
                .HasColumnName("Description_E");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Type).WithMany(p => p.ClmInitiatives)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_CLM_Initiatives_CLM_LKP_InitiativeType");
        });

        modelBuilder.Entity<ClmLkpAttachmentCategory>(entity =>
        {
            entity.ToTable("CLM_LKP_AttachmentCategory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
        });

        modelBuilder.Entity<ClmLkpContentType>(entity =>
        {
            entity.ToTable("CLM_LKP_ContentType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpInitiativeType>(entity =>
        {
            entity.ToTable("CLM_LKP_InitiativeType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpMediaType>(entity =>
        {
            entity.ToTable("CLM_LKP_MediaType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpParentCategory>(entity =>
        {
            entity.ToTable("CLM_LKP_ParentCategory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
        });

        modelBuilder.Entity<ClmLkpProject>(entity =>
        {
            entity.ToTable("CLM_LKP_Project");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Icon).HasMaxLength(255);
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpReportAbuseReason>(entity =>
        {
            entity.ToTable("CLM_LKP_ReportAbuseReasons");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpRole>(entity =>
        {
            entity.ToTable("CLM_LKP_Role");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(64)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmLkpSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLM_L__3214EC270018106D");

            entity.ToTable("CLM_LKP_Source");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NameA)
                .HasMaxLength(255)
                .HasColumnName("Name_A");
            entity.Property(e => e.NameE)
                .HasMaxLength(64)
                .HasColumnName("Name_E");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
        });

        modelBuilder.Entity<ClmMediaViw>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CLM_Media_VIW");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MediaTypeId).HasColumnName("MediaTypeID");
            entity.Property(e => e.MediaTypeNameA).HasMaxLength(64);
            entity.Property(e => e.MediaTypeNameE).HasMaxLength(64);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
        });

        modelBuilder.Entity<ClmMedium>(entity =>
        {
            entity.ToTable("CLM_Media");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE)
                .HasMaxLength(4000)
                .HasColumnName("Description_E");
            entity.Property(e => e.MediaTypeId).HasColumnName("MediaTypeID");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");

            entity.HasOne(d => d.MediaType).WithMany(p => p.ClmMedia)
                .HasForeignKey(d => d.MediaTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_Media_CLM_LKP_MediaType");
        });

        modelBuilder.Entity<ClmNotification>(entity =>
        {
            entity.ToTable("CLM_Notification");

            entity.HasIndex(e => e.UserId, "IX_CLM_Notification_UserID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BodyA)
                .HasMaxLength(4000)
                .HasColumnName("Body_A");
            entity.Property(e => e.BodyE)
                .HasMaxLength(4000)
                .HasColumnName("Body_E");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.ClmNotifications).HasForeignKey(d => d.ParentCategoryId);

            entity.HasOne(d => d.User).WithMany(p => p.ClmNotifications).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ClmPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CLM_News");

            entity.ToTable("CLM_Posts");

            entity.HasIndex(e => e.UserId, "IX_CLM_Posts_UserID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE)
                .HasMaxLength(4000)
                .HasColumnName("Description_E");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.SourceLink).HasMaxLength(1024);
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Type).WithMany(p => p.ClmPosts)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_Posts_CLM_LKP_ContentType");

            entity.HasOne(d => d.User).WithMany(p => p.ClmPosts).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<ClmReport>(entity =>
        {
            entity.ToTable("CLM_Reports");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA)
                .HasMaxLength(4000)
                .HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE)
                .HasMaxLength(4000)
                .HasColumnName("Description_E");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.SourceId).HasColumnName("SourceID");
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");

            entity.HasOne(d => d.Source).WithMany(p => p.ClmReports)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_CLM_Reports_CLM_LKP_Source");
        });

        modelBuilder.Entity<ClmReportAbuse>(entity =>
        {
            entity.ToTable("CLM_ReportAbuses");

            entity.HasIndex(e => e.UserId, "IX_CLM_ReportAbuses_UserID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ReasonId).HasColumnName("ReasonID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.ClmReportAbuses)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_CLM_ReportAbuses_CLM_LKP_ParentCategory");

            entity.HasOne(d => d.ReasonNavigation).WithMany(p => p.ClmReportAbuses)
                .HasForeignKey(d => d.ReasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLM_ReportAbuses_CLM_LKP_ReportAbuseReasons");

            entity.HasOne(d => d.User).WithMany(p => p.ClmReportAbuses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CLM_ReportAbuses_AspNetUsers_UserID");
        });

        modelBuilder.Entity<ClmStaticContent>(entity =>
        {
            entity.ToTable("CLM_StaticContent");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DescriptionA).HasColumnName("Description_A");
            entity.Property(e => e.DescriptionE).HasColumnName("Description_E");
            entity.Property(e => e.Icon).HasMaxLength(255);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PageName).HasMaxLength(255);
            entity.Property(e => e.RouterLink).HasMaxLength(255);
            entity.Property(e => e.TitleA)
                .HasMaxLength(255)
                .HasColumnName("Title_A");
            entity.Property(e => e.TitleE)
                .HasMaxLength(255)
                .HasColumnName("Title_E");
        });

        modelBuilder.Entity<ClmUser>(entity =>
        {
            entity.ToTable("CLM_User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccessToken).HasMaxLength(1024);
            entity.Property(e => e.ActivationCode).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastActivityIp)
                .HasMaxLength(50)
                .HasColumnName("LastActivityIP");
            entity.Property(e => e.LikedPosts).HasMaxLength(2048);
            entity.Property(e => e.Mobile).HasMaxLength(64);
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.ProfilePicture).HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasMaxLength(1024);
            entity.Property(e => e.RegistrationIp)
                .HasMaxLength(50)
                .HasColumnName("RegistrationIP");
            entity.Property(e => e.UserName).HasMaxLength(64);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
