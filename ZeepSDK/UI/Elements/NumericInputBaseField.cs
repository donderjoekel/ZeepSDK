using UnityEngine;
using UnityEngine.UIElements;

namespace ZeepSDK.UI.Elements
{
    public abstract class NumericInputBaseField<TNumber> : TextInputBaseField<TNumber>
    {
        public NumericInputBaseField()
            : this(null)
        {
        }

        public NumericInputBaseField(int maxLength, char maskChar)
            : this(null, maxLength, maskChar)
        {
        }

        public NumericInputBaseField(string label)
            : this(label, -1, '*')
        {
        }
        
        public NumericInputBaseField(
            string label,
            int maxLength,
            char maskChar)
            : base(label, maxLength, maskChar, new NumberInput())
        {
            AddToClassList(TextField.ussClassName);
            labelElement.AddToClassList(TextField.labelUssClassName);
            visualInput.AddToClassList(TextField.inputUssClassName);
            pickingMode = PickingMode.Ignore;
            SetValueWithoutNotify(default(TNumber));
            isDelayed = true;
        }
        
        public override TNumber value
        {
            get => base.value;
            set
            {
                base.value = value;
                text = ValueToString(rawValue);
            }
        }

        public override void SetValueWithoutNotify(TNumber newValue)
        {
            base.SetValueWithoutNotify(newValue);
            text = ValueToString(rawValue);
        }

        public override void OnViewDataReady()
        {
            base.OnViewDataReady();
            OverwriteFromViewData(this, GetFullHierarchicalViewDataKey());
            text = ValueToString(rawValue);
        }

        public sealed override string ValueToString(TNumber value)
        {
            return value.ToString();
        }
        
        

        private class NumberInput : TextInputBase
        {
            private NumericInputBaseField<TNumber> parentTextField => (NumericInputBaseField<TNumber>)parent;

            private TextEditorEngine EditorEngine => ((ITextInputField)this).editorEngine;

            public override TNumber StringToValue(string str)
            {
                return parentTextField.StringToValue(str);
            }

            public void SelectRange(int cursorIndex, int selectionIndex)
            {
                if (EditorEngine != null)
                {
                    EditorEngine.cursorIndex = cursorIndex;
                    EditorEngine.selectIndex = selectionIndex;
                }
            }

            public override void SyncTextEngine()
            {
                if (parentTextField != null)
                {
                    EditorEngine.isPasswordField = false;
                }

                base.SyncTextEngine();
            }

            public override void ExecuteDefaultActionAtTarget(EventBase evt)
            {
                base.ExecuteDefaultActionAtTarget(evt);

                if (evt == null)
                {
                    return;
                }

                if (evt.eventTypeId == KeyDownEvent.TypeId())
                {
                    KeyDownEvent kde = evt as KeyDownEvent;

                    if (!parentTextField.isDelayed || kde?.keyCode == KeyCode.KeypadEnter || kde?.keyCode == KeyCode.Return)
                    {
                        parentTextField.value = StringToValue(text);
                    }

                    if (kde?.character == 3 ||    // KeyCode.KeypadEnter
                        kde?.character == '\n')   // KeyCode.Return
                    {
                        parent.Focus();
                        evt.StopPropagation();
                        evt.PreventDefault();
                    }
                }
                else if (evt.eventTypeId == ExecuteCommandEvent.TypeId())
                {
                    ExecuteCommandEvent commandEvt = evt as ExecuteCommandEvent;
                    string cmdName = commandEvt.commandName;
                    if (!parentTextField.isDelayed && (cmdName == EventCommandNames.Paste || cmdName == EventCommandNames.Cut))
                    {
                        parentTextField.value = StringToValue(text);
                    }
                }
                // Prevent duplicated navigation events, since we're observing KeyDownEvents instead
                else if (evt.eventTypeId == NavigationSubmitEvent.TypeId() ||
                         evt.eventTypeId == NavigationCancelEvent.TypeId() ||
                         evt.eventTypeId == NavigationMoveEvent.TypeId())
                {
                    evt.StopPropagation();
                    evt.PreventDefault();
                }
            }

            public override void ExecuteDefaultAction(EventBase evt)
            {
                base.ExecuteDefaultAction(evt);

                if (parentTextField.isDelayed && evt?.eventTypeId == BlurEvent.TypeId())
                {
                    parentTextField.value = StringToValue(text);
                }
            }
        }
    }
}
