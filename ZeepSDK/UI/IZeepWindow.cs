using UnityEngine;

namespace ZeepSDK.UI;

public interface IZeepWindow
{
    bool Modal { get; }
    int Id { get; }
    Rect Rect { get; set; }
    GUIContent Content { get; }
    void OnZeepWindowGUI(int id);
}
