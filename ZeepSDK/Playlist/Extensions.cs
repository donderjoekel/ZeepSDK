using JetBrains.Annotations;
using ZeepSDK.Playlist;

[PublicAPI]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
// ReSharper disable once CheckNamespace
public static class Extensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc cref="PlaylistApi.CreateEditor"/>
    public static IPlaylistEditor CreateEditor(this PlaylistSaveJSON playlist)
    {
        return PlaylistApi.CreateEditor(playlist);
    }
}
