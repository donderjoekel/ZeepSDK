﻿using JetBrains.Annotations;
#pragma warning disable CS1591

namespace ZeepSDK.LevelEditor;

[PublicAPI]
public delegate void EnteredTestModeDelegate();

[PublicAPI]
public delegate void EnteredLevelEditorDelegate();

[PublicAPI]
public delegate void ExitedLevelEditorDelegate();