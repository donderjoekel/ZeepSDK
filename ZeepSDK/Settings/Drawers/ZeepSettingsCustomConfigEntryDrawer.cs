using BepInEx.Configuration;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

internal class ZeepSettingsCustomConfigEntryDrawer : IZeepSettingsDrawer
{
    private readonly ConfigEntryBase _entry;
    private readonly string _label;
    private readonly ModSettingsConfigEntryDrawDelegate _draw;

    public ZeepSettingsCustomConfigEntryDrawer(
        ConfigEntryBase entry,
        string label,
        ModSettingsConfigEntryDrawDelegate draw)
    {
        _entry = entry;
        _label = label ?? entry.Definition.Key;
        _draw = draw;
    }

    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            var entryContext = new ModSettingsConfigEntryDrawContext(_entry, _label);
            _draw(gui, context, entryContext);
        }
    }
}
