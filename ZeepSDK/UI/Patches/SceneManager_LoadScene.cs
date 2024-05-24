using System;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(SceneManager), nameof(SceneManager.LoadScene), [typeof(string)])]
internal class SceneManager_LoadScene
{
    public static Action BeforeLoadScene;
    
    public static void Prefix(string sceneName)
    {
        BeforeLoadScene?.Invoke();
    }
}
