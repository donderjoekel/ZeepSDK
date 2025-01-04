using UnityEngine;

namespace ZeepSDK.UI;

internal class ZeepGUIDispatcher : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
    }

    public void OnGUI()
    {
        ZeepGUI.OnGUI();
    }
}