using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.UI.Elements;

namespace ZeepSDK.UI
{
    /// <summary>
    /// Allows you to customize the given visual element
    /// </summary>
    public delegate void ConfigureElementHandler(VisualElement element);

    /// <summary>
    /// Allows you to apply a custom style
    /// </summary>
    public delegate void ConfigureStyleHandler(IStyle style);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.Toggle"/> control gets its value updated
    /// </summary>
    public delegate void ToggleValueUpdatedHandler(bool previousValue, bool newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.TextField"/> control gets its value updated
    /// </summary>
    public delegate void TextFieldValueUpdatedHandler(string previousValue, string newValue);

    /// <summary>
    /// Allows you to react to when an <see cref="ZeepGUI.IntField"/> control gets its value updated
    /// </summary>
    public delegate void IntFieldValueUpdatedHandler(int previousValue, int newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.FloatField"/> control gets its value updated
    /// </summary>
    public delegate void FloatFieldValueUpdatedHandler(float previousValue, float newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.DoubleField"/> control gets its value updated
    /// </summary>
    public delegate void DoubleFieldValueUpdatedHandler(double previousValue, double newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.Dropdown"/> control gets its value updated 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void DropDownValueUpdatedHandler<in T>(T previousValue, T newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.Button"/> is clicked
    /// </summary>
    public delegate void ButtonClickHandler();

    /// <summary>
    /// Allows you to build a menu when a <see cref="ZeepGUI.DropdownButton"/> is clicked
    /// </summary>
    public delegate void BuildDropdownMenuHandler(GenericDropdownMenu menu);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUIWindow"/> has its position changed
    /// <seealso cref="ZeepGUI.Window"/>
    /// </summary>
    public delegate void WindowPositionUpdatedHandler(Rect newValue);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUIWindow"/> is closed
    /// <seealso cref="ZeepGUI.Window"/>
    /// </summary>
    public delegate void WindowClosedHandler();

    /// <summary>
    /// Allows you to provide custom formatting for a dropdown item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate string DropDownFormatHandler<in T>(T input);

    /// <summary>
    /// Allows you to react to when a <see cref="ZeepGUI.Scroll(ZeepSDK.UI.ScrollValueUpdatedHandler,ZeepSDK.UI.ConfigureElementHandler,ZeepSDK.UI.ConfigureStyleHandler)"/> is scrolled
    /// </summary>
    public delegate void ScrollValueUpdatedHandler(Vector2 value);
}
