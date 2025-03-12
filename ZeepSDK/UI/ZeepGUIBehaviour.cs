using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.Controls;
using ZeepSDK.Utilities;

namespace ZeepSDK.UI
{
    [PublicAPI]
    public abstract class ZeepGUIBehaviour : MonoBehaviour
    {
        private static PanelSettings _panelSettings;
        
        private UIDocument _uiDocument;
        private DisposableBag _bag;

        protected UIDocument UIDocument => _uiDocument;
        protected ZeepGUI ZeepGUI { get; private set; }

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

        protected abstract void BuildUi();

        protected void RebuildUi()
        {
            _uiDocument.rootVisualElement.Clear();
            BuildUi();
        }
    }
}
