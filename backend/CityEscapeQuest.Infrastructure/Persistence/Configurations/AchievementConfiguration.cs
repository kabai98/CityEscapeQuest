using CityEscapeQuest.Domain.Achievements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityEscapeQuest.Infrastructure.Persistence.Configurations;

internal sealed class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements");
        builder.HasKey(achievement => achievement.Id);
        builder.Property(achievement => achievement.Code).HasMaxLength(80).IsRequired();
        builder.HasIndex(achievement => achievement.Code).IsUnique();
        builder.Property(achievement => achievement.Name).HasMaxLength(160).IsRequired();
        builder.Property(achievement => achievement.Description).HasMaxLength(500);
    }
}