using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityEscapeQuest.Infrastructure.Persistence.Configurations;

internal sealed class MissionConfiguration : IEntityTypeConfiguration<Mission>
{
    private static readonly Guid DanubeMissionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid ClockmakerMissionId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

    public void Configure(EntityTypeBuilder<Mission> builder)
    {
        builder.ToTable("Missions");
        builder.HasKey(mission => mission.Id);
        builder.Property(mission => mission.Title).HasMaxLength(160).IsRequired();
        builder.Property(mission => mission.ShortDescription).HasMaxLength(500).IsRequired();
        builder.Property(mission => mission.PreviewImageUrl).HasMaxLength(1_000);
        builder.Property(mission => mission.Price).HasPrecision(18, 2);
        builder.Ignore(mission => mission.IsFree);
        builder.OwnsOne(mission => mission.StartLocation, owned =>
        {
            owned.Property(point => point.Latitude).HasColumnName("StartLatitude");
            owned.Property(point => point.Longitude).HasColumnName("StartLongitude");
        });

        builder.HasData(
            new
            {
                Id = DanubeMissionId,
                Title = "A Duna elveszett üzenete",
                ShortDescription = "Rövid, ingyenes városi nyomozás Budapest szívében.",
                MinimumPlayers = 2,
                RecommendedPlayers = 4,
                EstimatedDurationMinutes = 45,
                Price = (decimal?)null,
                PreviewImageUrl = "/images/missions/danube-message.jpg",
                StartRadiusMeters = 100,
                IsPublished = true
            },
            new
            {
                Id = ClockmakerMissionId,
                Title = "A rejtélyes órásmester",
                ShortDescription = "Ingyenes bevezető küldetés a belvárosban.",
                MinimumPlayers = 2,
                RecommendedPlayers = 3,
                EstimatedDurationMinutes = 30,
                Price = (decimal?)null,
                PreviewImageUrl = "/images/missions/clockmaker.jpg",
                StartRadiusMeters = 80,
                IsPublished = true
            });

        builder.OwnsOne(mission => mission.StartLocation).HasData(
            new { MissionId = DanubeMissionId, Latitude = 47.4979, Longitude = 19.0402 },
            new { MissionId = ClockmakerMissionId, Latitude = 47.5009, Longitude = 19.0456 });
    }
}