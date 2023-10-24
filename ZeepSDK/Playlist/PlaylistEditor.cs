using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using Newtonsoft.Json;
using ZeepkistNetworking;
using ZeepSDK.Utilities;

namespace ZeepSDK.Playlist;

internal class PlaylistEditor : IPlaylistEditor
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(PlaylistEditor));

    private readonly PlaylistSaveJSON playlist;

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

    public IReadOnlyList<OnlineZeeplevel> Levels => playlist.levels;

    public PlaylistEditor(PlaylistSaveJSON playlist)
    {
        this.playlist = playlist;
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
        if (playlist.levels.Any(x => x.UID == level.UID) && !allowDuplicate)
            return;

        playlist.levels.Add(level);
        playlist.amountOfLevels = playlist.levels.Count;
    }

    public void Save()
    {
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
            string playlistPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Zeepkist",
                "Playlists",
                playlist.name + ".zeeplist");

            File.WriteAllText(playlistPath, json);
        }
        catch (Exception e)
        {
            logger.LogError("Failed to save playlist: " + e);
        }
    }
}
