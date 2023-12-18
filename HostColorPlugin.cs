using BepInEx;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;

namespace OC2HostColor
{
    [BepInPlugin("dev.gua.overcooked.hostcolor", "Overcooked2 HostColor Plugin", "1.0")]
    [BepInProcess("Overcooked2.exe")]
    public class HostColorPlugin : BaseUnityPlugin
    {
        public static HostColorPlugin pluginInstance;
        private static Harmony patcher;

        public void Awake()
        {
            pluginInstance = this;
            patcher = new Harmony("dev.gua.overcooked.hostcolor");
            patcher.PatchAll(typeof(Patch));
            foreach (var patched in patcher.GetPatchedMethods())
                Log("Patched: " + patched.FullDescription());
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }

        public static void Log(string msg) { pluginInstance.Logger.LogInfo(msg); }
    }
}