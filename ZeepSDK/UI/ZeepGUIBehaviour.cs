using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.Controls;
using ZeepSDK.Utilities;

namespace ZeepSDK.UI
{
    /// <summary>
    /// A basic behaviour that allows you to easily make UI
    /// </summary>
    [PublicAPI]
    public abstract class ZeepGUIBehaviour : MonoBehaviour
    {
        private static PanelSettings _panelSettings;
        
        private UIDocument _uiDocument;
        private DisposableBag _bag;

        /// <summary>
        /// The UI Document for this behaviour
        /// </summary>
        protected UIDocument UIDocument => _uiDocument;
        /// <summary>
        /// The ZeepGUI instance to create controls with
        /// </summary>
        protected ZeepGUI ZeepGUI { get; private set; }

        /// <summary>
        /// When created, should this behaviour block all input
        /// </summary>
        /// <returns></returns>
        protected abstract bool BlocksInput();

        private void Awake()
        {
            _uiDocument = gameObject.AddComponent<UIDocument>();
            if (BlocksInput())
            {
                _bag = ControlsApi.DisableAllInput();
            }
            OnAwake();
        }

        private void OnDestroy()
        {
            _bag.Dispose();
            OnDestroyed();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnDestroyed()
        {
        }

        private void Start()
        {
            if (_panelSettings == null)
            {
                string directoryName = Path.GetDirectoryName(Plugin.Instance.Info.Location);
                string assetBundlePath = Path.Combine(directoryName, "zeepgui");
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                _panelSettings = assetBundle.LoadAsset<PanelSettings>("NerdGUIDefaultPanelSettings");
            }

            _uiDocument.panelSettings = _panelSettings;
            ZeepGUI = new ZeepGUI(_uiDocument.rootVisualElement);

            OnStart();
            BuildUi();
        }

        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Used to define the UI of this behaviour
        /// </summary>
        protected abstract void BuildUi();

        /// <summary>
        /// Used to recreate the UI of this behaviour
        /// </summary>
        protected void RebuildUi()
        {
            _uiDocument.rootVisualElement.Clear();
            BuildUi();
        }
    }
}
