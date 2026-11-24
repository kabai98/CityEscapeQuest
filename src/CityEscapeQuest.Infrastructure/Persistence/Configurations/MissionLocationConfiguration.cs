using CityEscapeQuest.Domain.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityEscapeQuest.Infrastructure.Persistence.Configurations;

internal sealed class MissionLocationConfiguration : IEntityTypeConfiguration<MissionLocation>
{
    public void Configure(EntityTypeBuilder<MissionLocation> builder)
    {
        builder.ToTable("MissionLocations");
        builder.HasKey(location => location.Id);
        builder.Ignore(location => location.Coordinates);
        builder.HasData(
            new { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Latitude = 47.4979, Longitude = 19.0402, RadiusMeters = 100 },
            new { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Latitude = 47.497913, Longitude = 19.054416, RadiusMeters = 120 });
    }
}
