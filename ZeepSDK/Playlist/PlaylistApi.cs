using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private const int MaximumPlaylistFiles = 1000;
    private const long MaximumPlaylistBytes = 2 * 1024 * 1024;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(PlaylistApi));
    private static readonly object cacheLock = new();
    private static Dictionary<string, PlaylistSaveJSON> playlistsByName;
    private static IReadOnlyList<PlaylistSaveJSON> playlistSnapshot = Array.Empty<PlaylistSaveJSON>();

    /// <summary>
    /// Gets a list of all playlists that have been saved to disk
    /// </summary>
    public static IReadOnlyList<PlaylistSaveJSON> GetPlaylists()
    {
        EnsureCache();
        lock (cacheLock)
            return playlistSnapshot;
    }

    /// <summary>Reloads playlists from disk and rebuilds the name index.</summary>
    public static void RefreshPlaylists()
    {
        string playlistsPath = PlaylistPath.DirectoryPath;
        Dictionary<string, PlaylistSaveJSON> refreshed = new(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(playlistsPath))
        {
            ReplaceCache(refreshed);
            return;
        }

        foreach (string path in Directory.EnumerateFiles(playlistsPath, "*.zeeplist").Take(MaximumPlaylistFiles))
        {
            string contents;
            try
            {
                if (new FileInfo(path).Length > MaximumPlaylistBytes)
                {
                    logger.LogWarning($"Skipping oversized playlist at {path}");
                    continue;
                }

                contents = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to read playlist at {path}: {e}");
                continue;
            }

            try
            {
                PlaylistSaveJSON playlistSaveJson = JsonConvert.DeserializeObject<PlaylistSaveJSON>(
                    contents,
                    new JsonSerializerSettings { MaxDepth = 64 });
                if (playlistSaveJson == null || string.IsNullOrWhiteSpace(playlistSaveJson.name))
                    continue;

                playlistSaveJson.levels ??= new List<ZeepkistNetworking.OnlineZeeplevel>();
                playlistSaveJson.amountOfLevels = playlistSaveJson.levels.Count;
                if (!refreshed.TryAdd(playlistSaveJson.name, playlistSaveJson))
                    logger.LogWarning($"Skipping duplicate playlist name '{playlistSaveJson.name}' at {path}");
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to deserialize playlist at {path}: {e}");
            }
        }

        ReplaceCache(refreshed);
    }

    /// <summary>
    /// Does a playlist exist or not
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    public static bool Exists(string name)
    {
        PlaylistPath.Resolve(name);
        EnsureCache();
        lock (cacheLock)
            return playlistsByName.ContainsKey(name);
    }

    /// <summary>
    /// Tries to get a playlist by name
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    /// <returns>A PlaylistSaveJSON or null if the playlist does not exist</returns>
    public static PlaylistSaveJSON GetPlaylist(string name)
    {
        PlaylistPath.Resolve(name);
        EnsureCache();
        lock (cacheLock)
            return playlistsByName.TryGetValue(name, out PlaylistSaveJSON playlist) ? playlist : null;
    }

    /// <summary>
    /// Creates a new playlist and saves it to disk immediately
    /// </summary>
    /// <param name="name">The name of the playlist</param>
    /// <returns>A new playlist or an existing playlist if one is found on disk</returns>
    public static PlaylistSaveJSON CreatePlaylist(string name)
    {
        PlaylistPath.Resolve(name);
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
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));
        return new PlaylistEditor(playlist);
    }

    internal static bool CanSaveAs(PlaylistSaveJSON playlist, string name)
    {
        EnsureCache();
        lock (cacheLock)
            return !playlistsByName.TryGetValue(name, out PlaylistSaveJSON existing) || ReferenceEquals(existing, playlist);
    }

    internal static string GetStoredName(PlaylistSaveJSON playlist)
    {
        EnsureCache();
        lock (cacheLock)
        {
            foreach (KeyValuePair<string, PlaylistSaveJSON> entry in playlistsByName)
            {
                if (ReferenceEquals(entry.Value, playlist))
                    return entry.Key;
            }
        }

        return playlist.name;
    }

    internal static void UpdateCacheAfterSave(PlaylistSaveJSON playlist, string previousName)
    {
        lock (cacheLock)
        {
            playlistsByName ??= new Dictionary<string, PlaylistSaveJSON>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(previousName))
                playlistsByName.Remove(previousName);
            playlistsByName[playlist.name] = playlist;
            RefreshSnapshot();
        }
    }

    private static void EnsureCache()
    {
        lock (cacheLock)
        {
            if (playlistsByName != null)
                return;
        }

        RefreshPlaylists();
    }

    private static void ReplaceCache(Dictionary<string, PlaylistSaveJSON> refreshed)
    {
        lock (cacheLock)
        {
            playlistsByName = refreshed;
            RefreshSnapshot();
        }
    }

    private static void RefreshSnapshot()
    {
        playlistSnapshot = new ReadOnlyCollection<PlaylistSaveJSON>(
            playlistsByName.Values.OrderBy(playlist => playlist.name, StringComparer.OrdinalIgnoreCase).ToArray());
    }
}
