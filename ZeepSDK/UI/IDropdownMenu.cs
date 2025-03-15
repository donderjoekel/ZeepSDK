using System;

namespace ZeepSDK.UI
{
    /// <summary>
    /// Interface for a dropdown menu
    /// </summary>
    public interface IDropdownMenu
    {
        /// <summary>
        /// Adds an item with the name and callback
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="action"></param>
        void AddItem(string itemName, Action action);

        /// <summary>
        /// Adds an item with the name and parameterized callback
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        void AddItem(string itemName, Action<object> action, object data);

        /// <summary>
        /// Adds a disabled item without a callback
        /// </summary>
        /// <param name="itemName"></param>
        void AddDisabledItem(string itemName);

        /// <summary>
        /// Adds a separator
        /// </summary>
        void AddSeparator();
    }
}
