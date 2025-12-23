// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using ZeepSDK.UI;
//
// namespace ZeepSDK.Settings;
//
// internal class KeyBindWindow : ZeepGUIWindow
// {
//     public Action<KeyCode> KeySelected;
//     
//     protected override string GetTitle()
//     {
//         return "Keybinding";
//     }
//
//     protected override Rect GetInitialRect()
//     {
//         return Centered(400, 200);
//     }
//
//     protected override void OnAwake()
//     {
//         base.OnAwake();
//
//         KeyBindWindow[] windows = FindObjectsOfType<KeyBindWindow>();
//         if (windows.Length == 1)
//             return;
//
//         List<KeyBindWindow> existingWindows = windows.Where(x => x != this).ToList();
//         foreach (KeyBindWindow keyBindWindow in existingWindows)
//         {
//             keyBindWindow.Close();
//         }
//     }
//
//     protected override void BuildWindowUi()
//     {
//         using (ZeepGUI.Container(onConfigureStyle: style =>
//                {
//                    style.flexGrow = 1;
//                }))
//         {
//             ZeepGUI.Label("Press any key...", onConfigureStyle: style =>
//             {
//                 style.unityTextAlign = TextAnchor.MiddleCenter;
//                 style.flexGrow = 1;
//                 style.fontSize = 20;
//             });
//         }
//     }
//
//     private void OnGUI()
//     {
//         if (Event.current.type != EventType.KeyDown) 
//             return;
//         if (Event.current.keyCode == KeyCode.None)
//             return;
//         Close();
//         KeySelected?.Invoke(Event.current.keyCode);
//     }
//
//     protected override bool BlocksInput()
//     {
//         return true;
//     }
// }
