using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using BepInEx.Logging;
using Newtonsoft.Json;
using ZeepkistNetworking;
using ZeepSDK.Utilities;

namespace ZeepSDK.Playlist;

internal class PlaylistEditor : IPlaylistEditor
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(PlaylistEditor));

    private readonly PlaylistSaveJSON playlist;
    private string savedName;

    public string Name
    {
        get => playlist.name;
        set => playlist.name = value;
    }

    public bool Shuffle
    {
        get => playlist.shufflePlaylist;
        set => playlist.shufflePlaylist = value;
    }

    public double RoundLength
    {
        get => playlist.roundLength;
        set => playlist.roundLength = value;
    }

    public IReadOnlyList<OnlineZeeplevel> Levels
        => new ReadOnlyCollection<OnlineZeeplevel>(playlist.levels);

    public PlaylistEditor(PlaylistSaveJSON playlist)
    {
        this.playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
        this.playlist.levels ??= new List<OnlineZeeplevel>();
        this.playlist.amountOfLevels = this.playlist.levels.Count;
        savedName = PlaylistApi.GetStoredName(playlist);
    }

    public void AddLevel(string uid, string author, string name, ulong workshopId, bool allowDuplicate = false)
    {
        AddLevel(new OnlineZeeplevel
            {
                Author = author,
                UID = uid,
                Name = name,
                WorkshopID = workshopId
            },
            allowDuplicate);
    }

    public void AddLevel(LevelScriptableObject level, bool allowDuplicate = false)
    {
        AddLevel(new OnlineZeeplevel
            {
                Author = level.Author,
                Name = level.Name,
                UID = level.UID,
                WorkshopID = level.WorkshopID
            },
            allowDuplicate);
    }

    public void AddLevel(OnlineZeeplevel level, bool allowDuplicate = false)
    {
        if (level == null)
            throw new ArgumentNullException(nameof(level));
        if (playlist.levels.Any(x => x.UID == level.UID) && !allowDuplicate)
            return;

        playlist.levels.Add(level);
        playlist.amountOfLevels = playlist.levels.Count;
    }

    public void Save()
    {
        string newName = playlist.name;
        string newPath;
        try
        {
            newPath = PlaylistPath.Resolve(newName);
            if (!PlaylistApi.CanSaveAs(playlist, newName))
                throw new InvalidOperationException($"A playlist named '{newName}' already exists.");
        }
        catch (Exception e)
        {
            logger.LogError("Failed to validate playlist name: " + e);
            return;
        }

        playlist.amountOfLevels = playlist.levels.Count;
        string json;

        try
        {
            json = JsonConvert.SerializeObject(playlist);
        }
        catch (Exception e)
        {
            logger.LogError("Failed to serialize playlist: " + e);
            return;
        }

        try
        {
            Directory.CreateDirectory(PlaylistPath.DirectoryPath);
            AtomicFile.WriteAllText(newPath, json);

            string previousName = savedName;
            if (!string.IsNullOrEmpty(previousName) &&
                !string.Equals(previousName, newName, StringComparison.OrdinalIgnoreCase))
            {
                string previousPath = PlaylistPath.Resolve(previousName);
                if (File.Exists(previousPath))
                    File.Delete(previousPath);
            }

            PlaylistApi.UpdateCacheAfterSave(playlist, previousName);
            savedName = newName;
        }
        catch (Exception e)
        {
            logger.LogError("Failed to save playlist: " + e);
        }
    }
}
