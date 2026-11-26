using CityEscapeQuest.Domain.Common;

namespace CityEscapeQuest.Domain.Achievements;

public sealed class Achievement
{
    private Achievement()
    {
    }

    public Achievement(Guid id, string code, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Achievement code and name are required.");
        }

        Id = id;
        Code = code.Trim();
        Name = name.Trim();
        Description = description.Trim();
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
}