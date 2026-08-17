using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeepSDK.ChatCommands;

internal sealed class RemoteCommandRateLimiter
{
    private const double Capacity = 3;
    private const double TokensPerSecond = 1;
    private const int MaximumTrackedPlayers = 256;
    private readonly Dictionary<ulong, PlayerState> players = new();

    public bool TryConsume(ulong playerId, double nowSeconds)
    {
        if (!players.TryGetValue(playerId, out PlayerState state))
        {
            if (players.Count >= MaximumTrackedPlayers)
            {
                ulong oldestPlayer = players.OrderBy(entry => entry.Value.LastSeenSeconds).First().Key;
                players.Remove(oldestPlayer);
            }

            state = new PlayerState(Capacity, nowSeconds);
            players.Add(playerId, state);
        }

        double elapsed = Math.Max(0, nowSeconds - state.LastSeenSeconds);
        state.Tokens = Math.Min(Capacity, state.Tokens + elapsed * TokensPerSecond);
        state.LastSeenSeconds = nowSeconds;

        if (state.Tokens < 1)
            return false;

        state.Tokens--;
        return true;
    }

    private sealed class PlayerState
    {
        public double Tokens { get; set; }
        public double LastSeenSeconds { get; set; }

        public PlayerState(double tokens, double lastSeenSeconds)
        {
            Tokens = tokens;
            LastSeenSeconds = lastSeenSeconds;
        }
    }
}
