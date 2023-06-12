using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard;

[PublicAPI]
public interface ILeaderboardTab
{
    void Enable(OnlineTabLeaderboardUI sender);
    void Disable();
    void GoToPreviousPage();
    void GoToNextPage();
    void Draw();
}
