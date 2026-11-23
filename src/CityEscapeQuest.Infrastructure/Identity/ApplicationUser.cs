using Microsoft.AspNetCore.Identity;

namespace CityEscapeQuest.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}