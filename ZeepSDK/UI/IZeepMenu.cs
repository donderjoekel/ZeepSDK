using System;
using UnityEngine;

namespace ZeepSDK.UI;

public interface IZeepMenu
{
    void AddItem(GUIContent content, Action onClick);
    void AddItem(string text, Action onClick);
}
