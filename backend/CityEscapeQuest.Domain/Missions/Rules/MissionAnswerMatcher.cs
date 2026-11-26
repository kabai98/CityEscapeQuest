using System.Security.Cryptography;
using System.Text;

namespace CityEscapeQuest.Domain.Missions.Rules;

internal static class MissionAnswerMatcher
{
    // TODO: Introduce an answer-type enum and typed submissions for text, image, and audio answers,
    // with a matching strategy for each type instead of assuming every answer is text.
    public static bool Matches(string expectedHash, string answer)
    {
        if (string.IsNullOrWhiteSpace(expectedHash) || string.IsNullOrWhiteSpace(answer))
        {
            return false;
        }

        var normalized = answer.Trim().ToUpperInvariant();
        // Preserve the existing MVP answer alias until missions support configurable aliases.
        if (normalized == "HAT")
        {
            normalized = "6";
        }

        var suppliedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(expectedHash),
            Convert.FromHexString(suppliedHash));
    }
}
