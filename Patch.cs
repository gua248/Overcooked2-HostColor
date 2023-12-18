using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Team17.Online;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

namespace OC2HostColor
{
    public static class Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CampaignKitchenLoaderManager), "AssignChefEntities")]
        public static bool CampaignKitchenLoaderManagerAssignChefEntitiesPatch(CampaignKitchenLoaderManager __instance, FastList<User> users)
        {
            if (users.Count == 1) return true;
            PlayerIDProvider[] array = PlayerIDProvider.s_AllProviders.ToArray();
            int color = HostColorSettings.color;
            if (color > array.Length || color > users.Count) return true;

            Array.Sort(array, (PlayerIDProvider x, PlayerIDProvider y) => x.GetID() - y.GetID());
            PlayerIDProvider playerIDProvider = array[color - 1];
            EntitySerialisationEntry entry = EntitySerialisationRegistry.GetEntry(playerIDProvider.gameObject);
            users._items[0].EntityID = entry.m_Header.m_uEntityID;
            int num = 1;
            while (num < users.Count && num < array.Length)
            {
                playerIDProvider = array[num == color - 1 ? 0 : num];
                entry = EntitySerialisationRegistry.GetEntry(playerIDProvider.gameObject);
                users._items[num].EntityID = entry.m_Header.m_uEntityID;
                num++;
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(T17TabPanel), "OnTabSelected")]
        public static void T17TabPanelOnTabSelectedPatch()
        {
            HostColorSettings.AddUI();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SelectorOption), "OnLeftPressed")]
        public static bool SelectorOptionOnLeftPressed(SelectorOption __instance)
        {
            if (__instance == HostColorSettings.hostColorOption)
            {
                HostColorSettings.OnLeftPressed();
                HostColorSettings.SetText();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SelectorOption), "OnRightPressed")]
        public static bool SelectorOptionOnRightPressed(SelectorOption __instance)
        {
            if (__instance == HostColorSettings.hostColorOption)
            {
                HostColorSettings.OnRightPressed();
                HostColorSettings.SetText();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SelectorOption), "SyncUIWithOption")]
        public static bool SelectorOptionSyncUIWithOptionPatch(SelectorOption __instance)
        {
            if (__instance == HostColorSettings.hostColorOption)
            {
                HostColorSettings.SetText();
                return false;
            }
            return true;
        }
    }
}
