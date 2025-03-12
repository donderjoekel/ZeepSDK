using System;
using UnityEngine.UIElements;

namespace ZeepSDK.UI
{
    public static class VisualElementExtensions
    {
        public static void RegisterHierarchyChanged(this VisualElement visualElement, Action<VisualElement, int> onHierarchyChanged)
        {
            if (visualElement.panel == null)
            {
                visualElement.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            }
            else
            {
                visualElement.elementPanel.hierarchyChanged += HierarchyChanged;
            }

            void AttachedToPanel(AttachToPanelEvent evt)
            {
                visualElement.UnregisterCallback<AttachToPanelEvent>(AttachedToPanel);
                visualElement.elementPanel.hierarchyChanged += HierarchyChanged;
            }

            void HierarchyChanged(VisualElement element, HierarchyChangeType changeType)
            {
                onHierarchyChanged?.Invoke(element, (int)changeType);
            }
        }
    }
}
