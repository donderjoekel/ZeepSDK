using UnityEngine;

namespace ZeepSDK.UI
{
    public abstract class ZeepGUIWindow : ZeepGUIBehaviour
    {
        public static TWindow Open<TWindow>(bool useExistingIfAvailable)
            where TWindow : ZeepGUIWindow
        {
            if (useExistingIfAvailable)
            {
                TWindow existing = FindObjectOfType<TWindow>();
                if (existing != null)
                {
                    existing._rect = existing.GetInitialRect();
                    existing.UIDocument.rootVisualElement.BringToFront();
                    return existing;
                }
            }
            
            GameObject windowContainer = new(nameof(TWindow));
            TWindow window = windowContainer.AddComponent<TWindow>();
            return window;
        }
        
        protected abstract string GetTitle();
        protected abstract Rect GetInitialRect();

        private string _title;
        private Rect _rect;

        public Rect Rect => _rect;

        protected Rect Centered(float width, float height)
        {
            return Centered(new Vector2(width, height));
        }

        protected Rect Centered(Vector2 size)
        {
            Vector2 center = new(Screen.width / 2f, Screen.height / 2f);
            return new Rect(center - (size / 2), size);
        }
        
        protected sealed override void BuildUi()
        {
            _title = GetTitle();
            _rect = GetInitialRect();
            RebuildWindowUi();
        }

        protected void RebuildWindowUi()
        {
            UIDocument.rootVisualElement.Clear();
            using (ZeepGUI.Window(_title, _rect, value => _rect = value, Close))
            {
                BuildWindowUi();
            }
        }

        protected abstract void BuildWindowUi();

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
