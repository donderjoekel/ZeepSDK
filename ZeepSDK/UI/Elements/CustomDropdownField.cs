using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ZeepSDK.UI.Elements
{
    public class CustomDropdownField<TItem> : DropdownField
    {
        private readonly List<TItem> _choices;
        private readonly Dictionary<string, TItem> _formattedToItem;
        private readonly DropDownFormatHandler<TItem> _formatItem;

        public CustomDropdownField(string label, IEnumerable<TItem> choices, TItem initialChoice, DropDownFormatHandler<TItem> formatItem, UI.ZeepGUI.StyleMode styleMode)
            : base(label)
        {
            _choices = choices.ToList();
            _formatItem = formatItem;
            _formattedToItem = new Dictionary<string, TItem>();

            List<string> formattedChoices = new();
            foreach (TItem choice in _choices)
            {
                string formatted = _formatItem(choice);
                formattedChoices.Add(formatted);
                _formattedToItem.Add(formatted, choice);
            }

            m_Choices = formattedChoices;
            SetValueWithoutNotify(formatItem(initialChoice));

            RegisterCallback<ChangeEvent<string>>(OnValueChanged);

            createMenuCallback = () =>
            {
                GenericDropdownMenu genericDropdownMenu = new();
                genericDropdownMenu.menuContainer.AddToClassList("dropdown-menu");
                genericDropdownMenu.menuContainer.AddToClassList(styleMode.ToString().ToLower());
                return genericDropdownMenu;
            };
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            using (ChangeEvent<TItem> pooled =
                   ChangeEvent<TItem>.GetPooled(_formattedToItem[evt.previousValue], _formattedToItem[evt.newValue]))
            {
                SendEvent(pooled);
            }
        }
    }
}
