using JetBrains.Annotations;

namespace ZeepSDK.Playlist;

/// <summary>
/// Extensions for playlists
/// </summary>
[PublicAPI]
public static class PlaylistExtensions
{
    /// <inheritdoc cref="PlaylistApi.CreateEditor"/>
    public static IPlaylistEditor CreateEditor(this PlaylistSaveJSON playlist)
    {
        return PlaylistApi.CreateEditor(playlist);
    }
}
