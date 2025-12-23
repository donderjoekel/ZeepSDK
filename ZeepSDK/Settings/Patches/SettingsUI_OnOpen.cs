using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using ZeepSDK.UI;
using Object = UnityEngine.Object;

namespace ZeepSDK.Settings.Patches;

[HarmonyPatch(typeof(SettingsUI), nameof(SettingsUI.OnOpen))]
internal class SettingsUI_OnOpen
{
    private static FieldInfo UnityEventBase__m_Calls;
    private static FieldInfo UnityEventBase__m_PersistentCalls;
    private static MethodInfo InvokableCallList__Clear;
    private static MethodInfo InvokableCallList__ClearPersistent;
    private static MethodInfo PersistentCallGroup__Clear;
    
    [UsedImplicitly]
    private static void Postfix(SettingsUI __instance)
    {
        Transform sidePanelTransform = __instance.transform.Find("Zeepkist Settings")
            .Find("MENU > Overview")
            .Find("Side Panel");
        Transform inputBindingTransform = sidePanelTransform.Find("Input Binding");
        GenericButton inputBindingButton = inputBindingTransform.GetComponent<GenericButton>();

        GenericButton modsButton = Object.Instantiate(inputBindingButton, sidePanelTransform);

        RectTransform rTransform = (RectTransform)modsButton.transform;
        rTransform.anchorMin = new Vector2(0.1f, 0.17f);
        rTransform.anchorMax = new Vector2(0.9f, 0.25f);
        rTransform.offsetMin = new Vector2(0, 0);
        rTransform.offsetMax = new Vector2(0, 0);

        modsButton.GetComponentInChildren<TMP_Text>().text = "Mods";

        RemoveListeners(modsButton.onClick);
        RemoveListeners(modsButton.onHover);
        RemoveListeners(modsButton.onLeft);
        RemoveListeners(modsButton.onRight);
        modsButton.onClick.AddListener(() =>
        {
            SettingsApi.OpenModSettings();
            __instance.Close(true);
        });

        inputBindingButton.down = modsButton;

        GenericButton[] indicatorButtons = __instance.IndicatorButtons;
        Array.Resize(ref indicatorButtons, indicatorButtons.Length + 1);
        indicatorButtons[indicatorButtons.Length - 1] = modsButton;
        __instance.IndicatorButtons = indicatorButtons;
    }

    private static void RemoveListeners(UnityEventBase evt)
    {
        evt.RemoveAllListeners();
        ClearCals(evt);
        ClearPersistentCalls(evt);
        //evt.m_Calls.Clear();
        //evt.m_Calls.ClearPersistent();
        //evt.m_PersistentCalls.Clear();
        return;
        
        void ClearCals(UnityEventBase evt)
        {
            if (UnityEventBase__m_Calls == null)
            {
                UnityEventBase__m_Calls =
                    typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic);
                Type invokableCallListType = typeof(UnityEventBase).Assembly.GetTypes()
                    .FirstOrDefault(x => x.Name == "InvokableCallList");
                InvokableCallList__Clear =
                    invokableCallListType.GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                InvokableCallList__ClearPersistent = invokableCallListType.GetMethod("ClearPersistent",
                    BindingFlags.Instance | BindingFlags.Public);
            }


            object invocableCallList = UnityEventBase__m_Calls.GetValue(evt);
            InvokableCallList__Clear.Invoke(invocableCallList, null);
            InvokableCallList__ClearPersistent.Invoke(invocableCallList, null);
        }

        void ClearPersistentCalls(UnityEventBase evt)
        {
            if (UnityEventBase__m_PersistentCalls == null)
            {
                UnityEventBase__m_PersistentCalls =
                    typeof(UnityEventBase).GetField("m_PersistentCalls",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                Type persistentCallGroupType = typeof(UnityEventBase).Assembly.GetTypes()
                    .FirstOrDefault(x => x.Name == "PersistentCallGroup");
                PersistentCallGroup__Clear =
                    persistentCallGroupType.GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            }
            
            object persistentCallGroup = UnityEventBase__m_PersistentCalls.GetValue(evt);
            PersistentCallGroup__Clear.Invoke(persistentCallGroup, null);
        }
    }
}
