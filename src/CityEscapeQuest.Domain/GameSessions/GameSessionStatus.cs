namespace CityEscapeQuest.Domain.GameSessions;

public enum GameSessionStatus
{
    Lobby = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Expired = 5
}