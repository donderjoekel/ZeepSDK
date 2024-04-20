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
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(PlaylistEditor));

    private readonly PlaylistSaveJSON _playlist;

    public string Name
    {
        get => _playlist.name;
        set => _playlist.name = value;
    }

    public bool Shuffle
    {
        get => _playlist.shufflePlaylist;
        set => _playlist.shufflePlaylist = value;
    }

    public double RoundLength
    {
        get => _playlist.roundLength;
        set => _playlist.roundLength = value;
    }

    public IReadOnlyList<OnlineZeeplevel> Levels => _playlist.levels;

    public PlaylistEditor(PlaylistSaveJSON playlist)
    {
        _playlist = playlist;
    }

    public void AddLevel(string uid, string author, string name, ulong workshopId, bool allowDuplicate = false)
    {
        AddLevel(new OnlineZeeplevel { Author = author, UID = uid, Name = name, WorkshopID = workshopId },
            allowDuplicate);
    }

    public void AddLevel(LevelScriptableObject level, bool allowDuplicate = false)
    {
        AddLevel(
            new OnlineZeeplevel
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
        if (_playlist.levels.Any(x => x.UID == level.UID) && !allowDuplicate)
        {
            return;
        }

        _playlist.levels.Add(level);
        _playlist.amountOfLevels = _playlist.levels.Count;
    }

    public void Save()
    {
        string json;

        try
        {
            json = JsonConvert.SerializeObject(_playlist);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to serialize playlist: " + e);
            return;
        }

        try
        {
            string playlistPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Zeepkist",
                "Playlists",
                _playlist.name + ".zeeplist");

            File.WriteAllText(playlistPath, json);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to save playlist: " + e);
        }
    }
}
