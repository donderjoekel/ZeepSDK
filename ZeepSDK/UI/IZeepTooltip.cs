namespace ZeepSDK.UI;

/// <summary>
/// Interface for tooltip providers that supply tooltip information
/// </summary>
public interface IZeepTooltip
{
    /// <summary>
    /// Gets a value indicating whether the mouse is currently over the tooltip target
    /// </summary>
    bool IsOver { get; }
    
    /// <summary>
    /// Gets the text content to display in the tooltip
    /// </summary>
    string Content { get; }
}