using BepInEx;
using HarmonyLib;
using OC2HostColor.Extension;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OC2HostColor
{
    public static class HostColorSettings
    {
        public static int color = 1;
        private static FrontendOptionsMenu gameSettingsMenu = null;
        private static FrontendOptionsMenu modSettingsMenu = null;
        public static SelectorOption hostColorOption = null;

        public static void OnLeftPressed()
        {
            color--;
            if (color == 0) color = 4;
            GameUtils.TriggerAudio(GameOneShotAudioTag.UIHighlight, hostColorOption.gameObject.layer);
        }

        public static void OnRightPressed()
        {
            color++;
            if (color == 5) color = 1;
            GameUtils.TriggerAudio(GameOneShotAudioTag.UIHighlight, hostColorOption.gameObject.layer);
        }

        public static void SetText()
        {
            string text;
            if (Localization.GetLanguage() == SupportedLanguages.Chinese)
                text = new string[] { "小蓝", "小红", "小绿", "小黄" }[color - 1];
            else
                text = new string[] { "Blue", "Red", "Green", "Yellow" }[color - 1];
            hostColorOption.SetText(text);
        }

        private static void AddHostColorSettingsUI()
        {
            if (modSettingsMenu == null || hostColorOption != null) return;
            GameObject hostColorOptionObj = GameObject.Instantiate(gameSettingsMenu.transform.GetChild(0).Find("ContentPC").GetChild(1).GetChild(0).GetChild(2).gameObject);
            hostColorOptionObj.name = "HostColor";
            hostColorOptionObj.transform.SetParent(modSettingsMenu.transform.GetChild(0).Find("ContentPC").GetChild(1).GetChild(0), false);
            T17Text text = hostColorOptionObj.transform.GetChild(0).GetComponent<T17Text>();
            if (Localization.GetLanguage() == SupportedLanguages.Chinese)
            {
                text.text = "主机位置";
                text.m_LocalizationTag = "\"主机位置\"";
            }
            else
            {
                text.text = "Host Color";
                text.m_LocalizationTag = "\"Host Color\"";
            }
            hostColorOption = hostColorOptionObj.GetComponent<SelectorOption>();
            hostColorOption.set_m_Option(null);
            hostColorOption.set_m_OptionType((OptionsData.OptionType)(-1));
            modSettingsMenu.set_m_SyncOptions(modSettingsMenu.get_m_SyncOptions().AddToArray(hostColorOption));
            GamepadUser user = GameUtils.RequireManager<PlayerManager>().GetUser(EngagementSlot.One);
            if (user != null)
            {
                T17EventSystem eventSystemForGamepadUser = T17EventSystemsManager.Instance.GetEventSystemForGamepadUser(user);
                if (eventSystemForGamepadUser != null)
                    hostColorOption.get_m_ButtonToHaveFocusOnForLeftRight().SetEventSystem(eventSystemForGamepadUser);
            }
        }

        private static void AddModSettingsUI()
        {
            GameObject root = GameObject.Find("FrontendRootMenu");
            if (root == null) return;
            FrontendRootMenu frontendRootMenu = root.GetComponent<FrontendRootMenu>();
            gameSettingsMenu = root.transform.GetChild(1).GetChild(0).Find("GameOptions").GetComponent<FrontendOptionsMenu>();
            modSettingsMenu = root.transform.GetChild(1).GetChild(0).Find("ModOptions")?.GetComponent<FrontendOptionsMenu>();
            if (modSettingsMenu != null) return;

            Transform settingsTab = root.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(4);
            float dh = settingsTab.GetChild(0).GetComponent<RectTransform>().rect.height;
            RectTransform rect = settingsTab.GetComponent<RectTransform>();
            float h = rect.rect.height;
            rect.offsetMin += new Vector2(0, -dh);
            float py = rect.pivot.y;
            rect.pivot = new Vector2(rect.pivot.x, (h * py + dh) / (h + dh));
            T17Button modSettingsButton = GameObject.Instantiate(settingsTab.GetChild(0).gameObject).GetComponent<T17Button>();
            modSettingsButton.gameObject.name = "ModSettings";
            modSettingsButton.transform.SetParent(settingsTab, false);
            modSettingsButton.transform.SetSiblingIndex(3);

            T17Button credits = settingsTab.GetChild(2).GetComponent<T17Button>();
            Navigation navigation1 = credits.navigation;
            navigation1.selectOnDown = modSettingsButton;
            credits.navigation = navigation1;
            Navigation navigation2 = modSettingsButton.navigation;
            navigation2.selectOnDown = null;
            navigation2.selectOnUp = credits;
            modSettingsButton.navigation = navigation2;

            T17Text text = modSettingsButton.transform.GetChild(2).GetComponent<T17Text>();
            if (Localization.GetLanguage() == SupportedLanguages.Chinese)
            {
                text.text = "MOD";
                text.m_LocalizationTag = "\"MOD\"";
            }
            else
            {
                text.text = "MODs";
                text.m_LocalizationTag = "\"MODs\"";
            }
            settingsTab.GetChild(4).localPosition += new Vector3(0, -dh, 0);

            GameObject modSettingsMenuObj = GameObject.Instantiate(gameSettingsMenu.gameObject);
            modSettingsMenuObj.name = "ModOptions";
            modSettingsMenuObj.SetActive(false);
            modSettingsMenuObj.transform.SetParent(gameSettingsMenu.transform.parent, false);
            modSettingsMenuObj.transform.SetSiblingIndex(2);
            modSettingsMenu = modSettingsMenuObj.GetComponent<FrontendOptionsMenu>();
            text = modSettingsMenu.transform.GetChild(0).Find("HeaderBacker").GetChild(0).GetComponent<T17Text>();
            if (Localization.GetLanguage() == SupportedLanguages.Chinese)
            {
                text.text = "MOD设定";
                text.m_LocalizationTag = "\"MOD设定\"";
            }
            else
            {
                text.text = "MOD SETTINGS";
                text.m_LocalizationTag = "\"MOD SETTINGS\"";
            }
            Transform settingsBody = modSettingsMenu.transform.GetChild(0);
            settingsBody.Find("Cancel").gameObject.Destroy();
            settingsBody.Find("Confirm").gameObject.Destroy();
            settingsBody.Find("ContentConsole").gameObject.Destroy();
            settingsBody.Find("VersionNumber").gameObject.Destroy();
            Transform content = settingsBody.Find("ContentPC").GetChild(1).GetChild(0);
            for (int i = content.childCount - 1; i >= 0; i--)
                content.GetChild(i).gameObject.Destroy();
            rect = settingsBody.Find("ContentPC").GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
            modSettingsMenu.DoSingleTimeInitialize();
            modSettingsMenu.Hide(true, false);
            modSettingsMenu.set_m_ConsoleTopSelectable(null);
            modSettingsMenu.set_m_SyncOptions(new ISyncUIWithOption[0]);
            modSettingsMenu.set_m_VersionString(null);
            modSettingsMenu.OnShow = (BaseMenuBehaviour.BaseMenuBehaviourEvent)Delegate.Combine(modSettingsMenu.OnShow, new BaseMenuBehaviour.BaseMenuBehaviourEvent(frontendRootMenu.OnMenuShow));
            modSettingsMenu.OnHide = (BaseMenuBehaviour.BaseMenuBehaviourEvent)Delegate.Combine(modSettingsMenu.OnHide, new BaseMenuBehaviour.BaseMenuBehaviourEvent(frontendRootMenu.OnMenuHide));

            Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
            buttonClickedEvent.AddListener(delegate ()
            {
                GamepadUser user = frontendRootMenu.get_m_CurrentGamepadUser();
                modSettingsMenu.Show(user, frontendRootMenu, root, false);
            });
            modSettingsButton.onClick = buttonClickedEvent;
        }

        public static void AddUI()
        {
            AddModSettingsUI();
            AddHostColorSettingsUI();
        }
    }
}