using CityEscapeQuest.Domain.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityEscapeQuest.Infrastructure.Persistence.Configurations;

internal sealed class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.ToTable("GameSessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.OwnerUserId).HasMaxLength(450).IsRequired();
        builder.Property(session => session.JoinCodeHash).HasMaxLength(64).IsRequired();
        builder.Property(session => session.Status).HasConversion<string>().HasMaxLength(32);
        builder.HasMany(session => session.Participants)
            .WithOne()
            .HasForeignKey("GameSessionId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(session => session.Participants).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}