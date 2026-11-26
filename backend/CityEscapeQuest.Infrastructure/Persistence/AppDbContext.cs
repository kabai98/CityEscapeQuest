using CityEscapeQuest.Application.Abstractions;
using CityEscapeQuest.Domain.Achievements;
using CityEscapeQuest.Domain.GameSessions;
using CityEscapeQuest.Domain.Missions;
using CityEscapeQuest.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CityEscapeQuest.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<MissionLocation> MissionLocations => Set<MissionLocation>();
    public DbSet<Participant> GameSessionParticipants => Set<Participant>();
    public DbSet<RefreshTokenSession> RefreshTokenSessions => Set<RefreshTokenSession>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
