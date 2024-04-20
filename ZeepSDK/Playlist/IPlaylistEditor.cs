using System.Collections.Generic;
using JetBrains.Annotations;
using ZeepkistNetworking;

namespace ZeepSDK.Playlist;

/// <summary>
/// An editor for a playlist
/// </summary>
[PublicAPI]
public interface IPlaylistEditor
{
    /// <summary>
    /// The name of the playlist
    /// </summary>
    string Name
    {
        get; set;
    }

    /// <summary>
    /// The levels in the playlist
    /// </summary>
    IReadOnlyList<OnlineZeeplevel> Levels
    {
        get;
    }

    /// <summary>
    /// Should the playlist be shuffled?
    /// </summary>
    bool Shuffle
    {
        get; set;
    }

    /// <summary>
    /// The length of each round
    /// </summary>
    double RoundLength
    {
        get; set;
    }

    /// <summary>
    /// Adds a level to the playlist
    /// </summary>
    /// <param name="uid">The UID of the level</param>
    /// <param name="author">The author of the level</param>
    /// <param name="name">The name of the level</param>
    /// <param name="workshopId">The workshop id of the level</param>
    /// <param name="allowDuplicate">Can the level be added as a duplicate</param>
    void AddLevel(string uid, string author, string name, ulong workshopId, bool allowDuplicate = false);

    /// <summary>
    /// Adds a level to the playlist
    /// </summary>
    /// <param name="level">A LevelScriptableObject that represents a level</param>
    /// <param name="allowDuplicate">Can the level be added as a duplicate</param>
    void AddLevel(LevelScriptableObject level, bool allowDuplicate = false);

    /// <summary>
    /// Adds a level to the playlist
    /// </summary>
    /// <param name="level">An OnlineZeepLevel instance</param>
    /// <param name="allowDuplicate">Can the level be added as a duplicate</param>
    void AddLevel(OnlineZeeplevel level, bool allowDuplicate = false);

    /// <summary>
    /// Saves the playlist to disk
    /// </summary>
    void Save();
}
