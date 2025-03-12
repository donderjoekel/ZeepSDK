using System;
using UnityEngine.UIElements;

namespace ZeepSDK.UI
{
    internal class DropdownMenuWrapper : IDropdownMenu
    {
        private readonly GenericDropdownMenu _menu;
        
        public DropdownMenuWrapper(GenericDropdownMenu menu)
        {
            _menu = menu;
        }
        
        public void AddItem(string itemName, Action action)
        {
            _menu.AddItem(itemName, false, action);
        }

        public void AddItem(string itemName, Action<object> action, object data)
        {
            _menu.AddItem(itemName, false, action, data);
        }

        public void AddDisabledItem(string itemName)
        {
            _menu.AddDisabledItem(itemName, false);
        }

        public void AddSeparator()
        {
            _menu.AddSeparator(string.Empty);
        }
    }
}
