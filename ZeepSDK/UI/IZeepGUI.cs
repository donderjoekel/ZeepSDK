using JetBrains.Annotations;

namespace ZeepSDK.UI;

/// <summary>
/// An interface you can implement that allows you to receive GUI events from the ZeepSDK
/// </summary>
[PublicAPI]
public interface IZeepGUI
{
    /// <summary>
    /// The method that will be called when the GUI is being drawn
    /// </summary>
    void OnZeepGUI();
}