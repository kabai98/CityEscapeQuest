using CityEscapeQuest.Domain.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityEscapeQuest.Infrastructure.Persistence.Configurations;

internal sealed class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("GameSessionParticipants");
        builder.HasKey(participant => participant.Id);
        builder.Property(participant => participant.Id).ValueGeneratedNever();
        builder.Property(participant => participant.DisplayName).HasMaxLength(80).IsRequired();
        builder.Property(participant => participant.UserId).HasMaxLength(450);
        builder.Ignore(participant => participant.IsGuest);
        builder.HasIndex("GameSessionId", nameof(Participant.UserId));
    }
}
