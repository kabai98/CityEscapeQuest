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
        builder.Property(mission => mission.StoryIntro).HasMaxLength(2_000);
        builder.Property(mission => mission.TaskPrompt).HasMaxLength(1_000);
        builder.Property(mission => mission.AnswerHash).HasMaxLength(64);
        builder.Property(mission => mission.CompletionText).HasMaxLength(2_000);
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
                IsPublished = true,
                StoryIntro = "",
                TaskPrompt = "",
                AnswerHash = "",
                CompletionText = ""
            },
            new
            {
                Id = ClockmakerMissionId,
                Title = "A rejtélyes órásmester",
                ShortDescription = "Az eltűnt órásmester utolsó üzenete a Deák térre vezet.",
                MinimumPlayers = 1,
                RecommendedPlayers = 3,
                EstimatedDurationMinutes = 30,
                Price = (decimal?)null,
                PreviewImageUrl = "/images/missions/clockmaker.jpg",
                StartRadiusMeters = 120,
                IsPublished = true,
                StoryIntro = "Budapest központi órái 23:47-nél megálltak. Varga Áron órásmester eltűnt, de egy rézlapot hagyott hátra: a város csak akkor indul újra, ha megtaláljátok a helyreállító számot.",
                TaskPrompt = "Három föld alatti vonal találkozik ezen a téren. Add össze a vonalak számait, és írd be az eredményt számjeggyel vagy betűvel.",
                AnswerHash = "E7F6C011776E8DB7CD330B54174FD76F7D0216B612387A5FFCFB81E6F0919683",
                CompletionText = "A rézlap fogaskerekei megmozdulnak. A 6-os kód helyes: az órák újra járnak, és az órásmester következő nyoma felvillan a szerkezet belsejében."
            });

        builder.OwnsOne(mission => mission.StartLocation).HasData(
            new { MissionId = DanubeMissionId, Latitude = 47.4979, Longitude = 19.0402 },
            new { MissionId = ClockmakerMissionId, Latitude = 47.497913, Longitude = 19.054416 });
    }
}
