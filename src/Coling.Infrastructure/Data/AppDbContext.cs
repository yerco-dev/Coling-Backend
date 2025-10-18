using Coling.Domain.Entities;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Entities.UsersManagement;
using Coling.Domain.Entities.WorkManagement;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coling.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<InstitutionType> InstitutionTypes { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<DegreeEducation> DegreeEducations { get; set; }
    public DbSet<ContinuingEducation> ContinuingEducations { get; set; }
    public DbSet<ProfessionalCertification> ProfessionalCertifications { get; set; }
    public DbSet<MemberEducation> MemberEducations { get; set; }
    public DbSet<WorkFieldCategory> WorkFieldCategories { get; set; }
    public DbSet<WorkField> WorkFields { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<WorkExperienceField> WorkExperienceFields { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Member)
            .WithOne(m => m.Person)
            .HasForeignKey<Member>(m => m.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Person)
            .WithOne()
            .HasForeignKey<User>(u => u.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Person>()
            .HasIndex(p => p.NationalId)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.MembershipCode)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.TitleNumber)
            .IsUnique();

        modelBuilder.Entity<Institution>()
            .HasOne(i => i.InstitutionType)
            .WithMany(it => it.Institutions)
            .HasForeignKey(i => i.InstitutionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkField>()
            .HasOne(wf => wf.WorkFieldCategory)
            .WithMany(wfc => wfc.WorkFields)
            .HasForeignKey(wf => wf.WorkFieldCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único compuesto para evitar duplicados en la relación many-to-many
        modelBuilder.Entity<WorkExperienceField>()
            .HasIndex(wef => new { wef.WorkExperienceId, wef.WorkFieldId })
            .IsUnique();

        modelBuilder.Entity<WorkExperienceField>()
            .HasOne(wef => wef.WorkExperience)
            .WithMany(we => we.WorkExperienceFields)
            .HasForeignKey(wef => wef.WorkExperienceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkExperienceField>()
            .HasOne(wef => wef.WorkField)
            .WithMany(wf => wf.WorkExperienceFields)
            .HasForeignKey(wef => wef.WorkFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberEducation>()
            .HasOne(me => me.Member)
            .WithMany(m => m.Educations)
            .HasForeignKey(me => me.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MemberEducation>()
            .HasOne(me => me.Education)
            .WithMany()
            .HasForeignKey(me => me.EducationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkExperience>()
            .HasOne(we => we.Member)
            .WithMany(m => m.WorkExperiences)
            .HasForeignKey(we => we.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkExperience>()
            .HasOne(we => we.Institution)
            .WithMany(i => i.WorkExperiences)
            .HasForeignKey(we => we.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de relación Education → Institution
        modelBuilder.Entity<Education>()
            .HasOne(e => e.Institution)
            .WithMany()
            .HasForeignKey(e => e.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración TPT (Table Per Type) para Education
        modelBuilder.Entity<Education>()
            .ToTable("Educations");

        modelBuilder.Entity<DegreeEducation>()
            .ToTable("DegreeEducations");

        modelBuilder.Entity<ContinuingEducation>()
            .ToTable("ContinuingEducations");

        modelBuilder.Entity<ProfessionalCertification>()
            .ToTable("ProfessionalCertifications");

        // Configuración TPT (Table Per Type) para Party
        modelBuilder.Entity<Coling.Domain.Entities.PartyManagement.Party>()
            .ToTable("Parties");

        modelBuilder.Entity<Person>()
            .ToTable("Persons");

        modelBuilder.Entity<Institution>()
            .ToTable("Institutions");

        // Configuración de Owned Types para PartialDate
        modelBuilder.Entity<MemberEducation>()
            .OwnsOne(me => me.StartDate, sd =>
            {
                sd.Property(p => p.Year).HasColumnName("YearStarted").IsRequired();
                sd.Property(p => p.Month).HasColumnName("MonthStarted");
                sd.Property(p => p.Day).HasColumnName("DayStarted");
            });

        modelBuilder.Entity<MemberEducation>()
            .OwnsOne(me => me.EndDate, ed =>
            {
                ed.Property(p => p.Year).HasColumnName("YearCompleted");
                ed.Property(p => p.Month).HasColumnName("MonthCompleted");
                ed.Property(p => p.Day).HasColumnName("DayCompleted");
            });

        modelBuilder.Entity<WorkExperience>()
            .OwnsOne(we => we.StartDate, sd =>
            {
                sd.Property(p => p.Year).HasColumnName("StartYear").IsRequired();
                sd.Property(p => p.Month).HasColumnName("StartMonth");
                sd.Property(p => p.Day).HasColumnName("StartDay");
            });

        modelBuilder.Entity<WorkExperience>()
            .OwnsOne(we => we.EndDate, ed =>
            {
                ed.Property(p => p.Year).HasColumnName("EndYear");
                ed.Property(p => p.Month).HasColumnName("EndMonth");
                ed.Property(p => p.Day).HasColumnName("EndDay");
            });
    }
}
