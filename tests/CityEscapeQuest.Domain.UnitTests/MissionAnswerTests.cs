using CityEscapeQuest.Domain.Missions.ValueObjects;
using System.Security.Cryptography;
using System.Text;
using CityEscapeQuest.Domain.Missions;

namespace CityEscapeQuest.Domain.UnitTests;

public sealed class MissionAnswerTests
{
    [Theory]
    [InlineData("6")]
    [InlineData("hat")]
    [InlineData(" HAT ")]
    public void IsCorrectAnswer_AcceptsConfiguredClockmakerAnswer(string answer)
    {
        var answerHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("6")));
        var mission = new Mission(
            Guid.NewGuid(), "Mission", "Description", 1, 3, 30, null, "/preview.jpg",
            new GeoPoint(47.497913, 19.054416), 120, "Story", "Prompt", answerHash, "Complete");

        Assert.True(mission.IsCorrectAnswer(answer));
    }
}
