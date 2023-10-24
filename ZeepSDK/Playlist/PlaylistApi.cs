using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ZeepSDK.Utilities;

namespace ZeepSDK.Playlist;

/// <summary>
/// An API for interacting with playlists
/// </summary>
[PublicAPI]
public class PlaylistApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(PlaylistApi));

    /// <summary>
    /// Gets a list of all playlists that have been saved to disk
    /// </summary>
    public static IReadOnlyList<PlaylistSaveJSON> GetPlaylists()
    {
        string playlistsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeepkist",
            "Playlists");

        if (!Directory.Exists(playlistsPath))
            return Array.Empty<PlaylistSaveJSON>();

        List<PlaylistSaveJSON> playlists = new();
        string[] paths = Directory.GetFiles(playlistsPath, "*.zeeplist");

        foreach (string path in paths)
        {
            string contents;
            try
            {
                contents = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to read playlist at {path}: {e}");
                continue;
            }

            try
            {
                PlaylistSaveJSON playlistSaveJson = JsonConvert.DeserializeObject<PlaylistSaveJSON>(contents);
                playlists.Add(playlistSaveJson);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to deserialize playlist at {path}: {e}");
            }
        }

        return playlists;
    }

    /// <summary>
    /// Does a playlist exist or not
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    public static bool Exists(string name)
    {
        return GetPlaylists().Any(playlist => playlist.name == name);
    }

    /// <summary>
    /// Tries to get a playlist by name
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    /// <returns>A PlaylistSaveJSON or null if the playlist does not exist</returns>
    public static PlaylistSaveJSON GetPlaylist(string name)
    {
        return GetPlaylists().FirstOrDefault(playlist => playlist.name == name);
    }

    /// <summary>
    /// Creates a new playlist and saves it to disk immediately
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    /// <returns>A new playlist or an existing playlist if one is found on disk</returns>
    public static PlaylistSaveJSON CreatePlaylist(string name)
    {
        PlaylistSaveJSON existingPlaylist = GetPlaylist(name);

        if (existingPlaylist != null)
            return existingPlaylist;

        PlaylistSaveJSON playlist = new()
        {
            name = name
        };

        CreateEditor(playlist).Save();

        return playlist;
    }

    /// <summary>
    /// Returns a new editor for a playlist
    /// </summary>
    /// <param name="playlist">An existing playlist</param>
    /// <returns><see cref="IPlaylistEditor"/></returns>
    public static IPlaylistEditor CreateEditor(PlaylistSaveJSON playlist)
    {
        return new PlaylistEditor(playlist);
    }
}
