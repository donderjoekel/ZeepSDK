using System;

namespace ZeepSDK.UI
{
    public interface IDropdownMenu
    {
        void AddItem(string itemName, Action action);
        void AddItem(string itemName, Action<object> action, object data);
        void AddDisabledItem(string itemName);
        void AddSeparator();
    }
}
