using JetBrains.Annotations;
using ZeepkistNetworking;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// An item describing a level in a playlist
/// </summary>
[PublicAPI]
public class PlaylistItem
{
    /// <summary>
    /// The UID of the track
    /// </summary>
    public string Uid { get; }

    /// <summary>
    /// The workshop ID of the track
    /// </summary>
    public ulong WorkshopId { get; }

    /// <summary>
    /// The name of the track
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The author of the track
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistItem"/> class
    /// </summary>
    /// <param name="uid">The UID of the track</param>
    /// <param name="workshopId">The workshop ID of the track</param>
    /// <param name="name">The name of the track</param>
    /// <param name="author">The author of the track</param>
    public PlaylistItem(string uid, ulong workshopId, string name, string author)
    {
        Uid = uid;
        WorkshopId = workshopId;
        Name = name;
        Author = author;
    }

    internal OnlineZeeplevel ToOnlineZeepLevel()
    {
        return new OnlineZeeplevel
        {
            UID = Uid,
            WorkshopID = WorkshopId,
            Name = Name,
            Author = Author
        };
    }
}
