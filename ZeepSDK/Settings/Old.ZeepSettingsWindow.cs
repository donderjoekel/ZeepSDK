// using System;
// using System.Collections.Generic;
// using System.Linq;
// using BepInEx;
// using BepInEx.Bootstrap;
// using BepInEx.Configuration;
// using UnityEngine;
// using UnityEngine.UIElements;
// using ZeepSDK.UI;
//
// namespace ZeepSDK.Settings;
//
// internal class ZeepSettingsWindow : ZeepGUIWindow
// {
//     protected override string GetTitle()
//     {
//         return "Zeep Settings";
//     }
//
//     protected override Rect GetInitialRect()
//     {
//         return Centered(1280, 720);
//     }
//
//     private PluginInfo _selectedPlugin;
//     private Vector2 _listScroll;
//     private Vector2 _entriesScroll;
//
//     protected override void OnClose()
//     {
//         SettingsApi.DispatchWindowClosed();
//     }
//
//     protected override void BuildWindowUi()
//     {
//         List<PluginInfo> plugins = Chainloader.PluginInfos.Values
//             .Where(x => x.Instance.Config.Keys.Any(y =>
//                 !y.Key.StartsWith("[hide]", StringComparison.OrdinalIgnoreCase) &&
//                 !y.Key.StartsWith("[hidden]", StringComparison.OrdinalIgnoreCase)))
//             .OrderBy(x => x.Metadata.Name)
//             .ToList();
//
//         using (ZeepGUI.Horizontal())
//         {
//             BuildPluginsList(plugins);
//             BuildPluginOptions();
//         }
//     }
//
//     private void BuildPluginsList(List<PluginInfo> plugins)
//     {
//         using ZeepGUI.IZeepScope container = ZeepGUI.Vertical(onConfigureStyle: style =>
//         {
//             style.flexGrow = 0;
//             style.width = Length.Percent(25);
//         });
//         using ZeepGUI.IZeepScope scroll = ZeepGUI.Scroll(_listScroll, value => _listScroll = value);
//
//         foreach (PluginInfo plugin in plugins)
//         {
//             bool isSelectedPlugin = plugin == _selectedPlugin;
//
//             ZeepGUI.Button($"{plugin.Metadata.Name} ({plugin.Metadata.Version})", () =>
//             {
//                 PluginInfo previous = _selectedPlugin;
//                 _selectedPlugin = plugin;
//                 _entriesScroll = Vector2.zero;
//                 RebuildWindowUi();
//                 SettingsApi.DispatchTabChanged(previous, plugin);
//             }, styleMode: isSelectedPlugin ? ZeepGUI.StyleMode.Primary : ZeepGUI.StyleMode.Regular);
//         }
//     }
//
//     private void BuildPluginOptions()
//     {
//         if (_selectedPlugin == null)
//             return;
//
//         using ZeepGUI.IZeepScope container = ZeepGUI.Vertical(onConfigureStyle: style =>
//         {
//             style.width = Length.Percent(75);
//             // style.borderBottomColor = style.borderLeftColor = style.borderRightColor = style.borderTopColor = Color.red;
//             // style.borderBottomWidth = style.borderLeftWidth = style.borderRightWidth = style.borderTopWidth = 1;
//         });
//         using ZeepGUI.IZeepScope scroll = ZeepGUI.Scroll(_entriesScroll, value => _entriesScroll = value,
//             onConfigureStyle: style =>
//             {
//                 style.width = Length.Percent(100);
//             });
//
//         List<IGrouping<string, KeyValuePair<ConfigDefinition, ConfigEntryBase>>> groups =
//             _selectedPlugin
//                 .Instance.Config
//                 .GroupBy(x => x.Key.Section)
//                 .OrderBy(x => x.Key)
//                 .ToList();
//
//         ZeepGUI.Label(_selectedPlugin.Metadata.Name, styleMode: ZeepGUI.StyleMode.Primary, onConfigureStyle:
//             style =>
//             {
//                 style.fontSize = 48;
//                 style.unityTextAlign = TextAnchor.MiddleCenter;
//             });
//
//         BuildPluginGroups(groups);
//     }
//
//     private void BuildPluginGroups(List<IGrouping<string, KeyValuePair<ConfigDefinition, ConfigEntryBase>>> groups)
//     {
//         foreach (IGrouping<string, KeyValuePair<ConfigDefinition, ConfigEntryBase>> group in groups)
//         {
//             List<KeyValuePair<ConfigDefinition, ConfigEntryBase>> entries = group.ToList();
//             if (entries.All(x =>
//                     x.Key.Key.StartsWith("[hide]", StringComparison.OrdinalIgnoreCase) ||
//                     x.Key.Key.StartsWith("[hidden]", StringComparison.OrdinalIgnoreCase)))
//                 continue;
//
//             ZeepGUI.Label(group.Key, onConfigureStyle: style =>
//             {
//                 style.fontSize = 28;
//                 style.marginBottom = 8;
//                 style.unityTextAlign = TextAnchor.MiddleCenter;
//             });
//
//             BuildPluginEntries(entries);
//         }
//     }
//
//     private void BuildPluginEntries(List<KeyValuePair<ConfigDefinition, ConfigEntryBase>> entries)
//     {
//         foreach ((ConfigDefinition definition, ConfigEntryBase entry) in entries)
//         {
//             using ZeepGUI.IZeepScope container = ZeepGUI.Vertical(onConfigureStyle: style =>
//                    {
//                        style.flexGrow = 1;
//                    });
//
//             if (entry.Description.Description.StartsWith("[button]",
//                     StringComparison.OrdinalIgnoreCase))
//             {
//                 ZeepGUI.Button(definition.Key,
//                     () =>
//                     {
//                         entry.BoxedValue = !(bool)entry.BoxedValue;
//                         RebuildWindowUi();
//                     }, tooltip: entry.Description.Description.Replace("[button]", string.Empty, StringComparison.OrdinalIgnoreCase));
//                 continue;
//             }
//
//             ZeepGUI.Label(definition.Key, tooltip: entry.Description.Description);
//
//             if (entry.SettingType == typeof(KeyCode))
//             {
//                 using ZeepGUI.IZeepScope horizontal = ZeepGUI.Horizontal();
//                 ZeepGUI.Button(Enum.GetName(entry.SettingType, entry.BoxedValue), () =>
//                     {
//                         KeyBindWindow window = Open<KeyBindWindow>(false);
//                         window.KeySelected += code =>
//                         {
//                             entry.BoxedValue = code;
//                             RebuildWindowUi();
//                         };
//                     }, tooltip: entry.Description.Description,
//                     onConfigureStyle: style => style.flexGrow = 1);
//                 ZeepGUI.Button("Reset", () =>
//                 {
//                     entry.BoxedValue = entry.DefaultValue;
//                     RebuildWindowUi();
//                 });
//                 ZeepGUI.Button("Clear", () =>
//                 {
//                     entry.BoxedValue = KeyCode.None;
//                     RebuildWindowUi();
//                 });
//             }
//             else if (entry.SettingType.IsEnum)
//             {
//                 string[] enumNames = Enum.GetNames(entry.SettingType);
//                 string selectedEnumValue = Enum.GetName(entry.SettingType, entry.BoxedValue);
//                 ZeepGUI.Dropdown(string.Empty, enumNames, selectedEnumValue, input => input,
//                     (value, newValue) => { entry.BoxedValue = Enum.Parse(entry.SettingType, newValue); }, tooltip: entry.Description.Description);
//             }
//             else if (entry.SettingType == typeof(int))
//             {
//                 if (!DrawAsList<int>(entry))
//                 {
//                     ZeepGUI.IntField(string.Empty, (int)entry.BoxedValue,
//                         (value, newValue) => entry.BoxedValue = newValue, tooltip: entry.Description.Description);
//                 }
//             }
//             else if (entry.SettingType == typeof(float))
//             {
//                 if (!DrawAsList<float>(entry))
//                 {
//                     ZeepGUI.FloatField(string.Empty, (float)entry.BoxedValue,
//                         (value, newValue) => entry.BoxedValue = newValue, tooltip: entry.Description.Description);
//                 }
//             }
//             else if (entry.SettingType == typeof(double))
//             {
//                 if (!DrawAsList<double>(entry))
//                 {
//                     ZeepGUI.DoubleField(string.Empty, (float)entry.BoxedValue,
//                         (value, newValue) => entry.BoxedValue = newValue, tooltip: entry.Description.Description);
//                 }
//             }
//             else if (entry.SettingType == typeof(string))
//             {
//                 if (!DrawAsList<string>(entry))
//                 {
//                     ZeepGUI.TextField(string.Empty, (string)entry.BoxedValue,
//                         (value, newValue) => entry.BoxedValue = newValue, tooltip: entry.Description.Description);
//                 }
//             }
//             else if (entry.SettingType == typeof(bool))
//             {
//                 ZeepGUI.Toggle(string.Empty, (bool)entry.BoxedValue, (value, newValue) =>
//                     entry.BoxedValue = newValue, tooltip: entry.Description.Description);
//             }
//             else
//             {
//                 ZeepGUI.Label(entry.SettingType.Name + " not implement");
//             }
//         }
//     }
//
//     private bool DrawAsList<T>(ConfigEntryBase entry)
//         where T : IEquatable<T>
//     {
//         if (entry.Description.AcceptableValues == null)
//             return false;
//         if (entry.Description.AcceptableValues is not AcceptableValueList<T> list)
//             return false;
//
//         ZeepGUI.Dropdown(string.Empty, list.AcceptableValues, (T)entry.BoxedValue, input => input.ToString(),
//             (value, newValue) =>
//             {
//                 entry.BoxedValue = newValue;
//             }, tooltip: entry.Description.Description);
//         return true;
//     }
//
//     protected override bool BlocksInput()
//     {
//         return true;
//     }
// }