using System.Collections.Generic;
using JetBrains.Annotations;

#pragma warning disable CS1591

namespace ZeepSDK.LevelEditor;

[PublicAPI]
public delegate void EnteredTestModeDelegate();

[PublicAPI]
public delegate void EnteredLevelEditorDelegate();

[PublicAPI]
public delegate void ExitedLevelEditorDelegate();

[PublicAPI]
public delegate void LevelSavedDelegate();

[PublicAPI]
public delegate void LevelLoadedDelegate();

[PublicAPI]
public delegate void SelectionChangedDelegate(List<BlockProperties> selection);
