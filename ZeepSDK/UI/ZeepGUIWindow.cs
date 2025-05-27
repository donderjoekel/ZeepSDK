using UnityEngine;

namespace ZeepSDK.UI
{
    /// <summary>
    /// A base class that can be inherited to more easily create a window
    /// </summary>
    public abstract class ZeepGUIWindow : ZeepGUIBehaviour
    {
        /// <summary>
        /// Opens the window of the given type
        /// </summary>
        /// <param name="useExistingIfAvailable"></param>
        /// <typeparam name="TWindow"></typeparam>
        /// <returns></returns>
        public static TWindow Open<TWindow>(bool useExistingIfAvailable)
            where TWindow : ZeepGUIWindow
        {
            if (useExistingIfAvailable)
            {
                TWindow existing = FindObjectOfType<TWindow>();
                if (existing != null)
                {
                    existing._rect = existing.GetInitialRect();
                    existing.RebuildUi();
                    existing.UIDocument.rootVisualElement.BringToFront();
                    return existing;
                }
            }
            
            GameObject windowContainer = new(nameof(TWindow));
            TWindow window = windowContainer.AddComponent<TWindow>();
            return window;
        }
        
        /// <summary>
        /// Gets the title for the window
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTitle();
        
        /// <summary>
        /// Gets the initial rect (position/size) of the window
        /// </summary>
        /// <returns></returns>
        protected abstract Rect GetInitialRect();

        private string _title;
        private Rect _rect;

        /// <summary>
        /// The position and size of the window
        /// </summary>
        public Rect Rect => _rect;

        /// <summary>
        /// Creates a screen-centered rect
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected Rect Centered(float width, float height)
        {
            return Centered(new Vector2(width, height));
        }

        /// <summary>
        /// Creates a screen-centered rect
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected Rect Centered(Vector2 size)
        {
            Vector2 center = new(Screen.width / 2f, Screen.height / 2f);
            return new Rect(center - (size / 2), size);
        }
        
        /// <summary>
        /// Used to define the UI of this window
        /// </summary>
        protected sealed override void BuildUi()
        {
            _title = GetTitle();
            _rect = GetInitialRect();
            RebuildWindowUi();
        }

        /// <summary>
        /// Used to recreate the UI of this window
        /// </summary>
        protected void RebuildWindowUi()
        {
            UIDocument.rootVisualElement.Clear();
            using (ZeepGUI.Window(_title, _rect, value => _rect = value, Close))
            {
                BuildWindowUi();
            }
        }

        /// <summary>
        /// Used to define the UI of this window
        /// </summary>
        protected abstract void BuildWindowUi();

        /// <summary>
        /// Used to close the window
        /// </summary>
        public void Close()
        {
            OnClose();
            Destroy(gameObject);
        }

        /// <summary>
        /// Called right before the window closes
        /// </summary>
        protected virtual void OnClose()
        {
        }
    }
}
