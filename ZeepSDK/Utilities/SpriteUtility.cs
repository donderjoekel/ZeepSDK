using System;
using BepInEx.Logging;
using UnityEngine;

namespace ZeepSDK.Utilities;

/// <summary>
/// A class containing utilities that are related to <see cref="Sprite"/>
/// </summary>
public static class SpriteUtility
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(SpriteUtility));
    
    /// <summary>
    /// Creates a <see cref="Sprite"/> from a base64 string
    /// </summary>
    /// <returns>Either the converted sprite or a red sprite if the conversion failed</returns>
    public static Sprite FromBase64(string b64)
    {
        try
        {
            byte[] buffer = Convert.FromBase64String(b64);
            
            Texture2D texture = new(1, 1);
            texture.LoadImage(buffer);
            
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to create sprite from base64: {e}");
            return Sprite.Create(Texture2D.redTexture,
                new Rect(0, 0, Texture2D.redTexture.width, Texture2D.redTexture.height),
                Vector2.one * 0.5f);
        }
    }
}
