using UnityEngine;
using UnityEngine.UIElements;

namespace ZeepSDK.UI
{
    public delegate void ConfigureElementHandler(VisualElement element);
    public delegate void ConfigureStyleHandler(IStyle style);
    
    public delegate void ToggleValueUpdatedHandler(bool previousValue, bool newValue);
    public delegate void TextFieldValueUpdatedHandler(string previousValue, string newValue);
    public delegate void IntFieldValueUpdatedHandler(int previousValue, int newValue);
    public delegate void FloatFieldValueUpdatedHandler(float previousValue, float newValue);
    public delegate void DoubleFieldValueUpdatedHandler(double previousValue, double newValue);
    public delegate void DropDownValueUpdatedHandler<in T>(T previousValue, T newValue);

    public delegate void ButtonClickHandler();

    public delegate void BuildDropdownMenuHandler(GenericDropdownMenu menu);
    
    public delegate void WindowPositionUpdatedHandler(Rect newValue);
    public delegate void WindowClosedHandler();
    
    public delegate string DropDownFormatHandler<in T>(T input);

    public delegate void ScrollValueUpdatedHandler(Vector2 value);
}
