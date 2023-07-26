using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using qol_core;
using UnityEngine.UI;

namespace HealthColor
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("qol-core")]
    public class Plugin : BasePlugin
    {
        private static Plugin _instance;
        private static Mod _modInstance;

        ConfigEntry<string> Health;
        ConfigEntry<string> Damage;

        public static Image HealthColor;
        public static Image DamageColor;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            _instance = this;

            Health = Config.Bind(
                "Color",
                "Health",
                "0.46,1,0.44",
                "Color of the health part."
                );
            Damage = Config.Bind(
                "Color",
                "Damage",
                "1,0.8,1",
                "Color of the damage part."
                );

            _modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "Change the color of your health bar.", "LualtOfficial/HealthColor");

            Commands.RegisterCommand("color", "color [health|damage|color] [color]", "Change the color.", _modInstance, ColorCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(GameUI), nameof(GameUI.Awake))]
        [HarmonyPostfix]
        public static void GameUIAwake()
        {
            GameObject health = GameObject.Find("GameUI/Status/BottomRight/Tab0/Hp (1)/Circle");
            HealthColor = health.GetComponent<Image>();
            string[] healthColor = _instance.Health.Value.Split(",");

            GameObject damage = GameObject.Find("GameUI/Status/BottomRight/Tab0/Hp (1)/Inner");
            DamageColor = damage.GetComponent<Image>();
            string[] damageColor = _instance.Damage.Value.Split(",");

            HealthColor.color = new Color(float.Parse(healthColor[0]), float.Parse(healthColor[1]), float.Parse(healthColor[2]));
            DamageColor.color = new Color(float.Parse(damageColor[0]), float.Parse(damageColor[1]), float.Parse(damageColor[2]));
        }

        public static bool ColorCommand(List<string> arguments)
        {
            int argCount = arguments.Count;

            if (argCount == 3)
            {
                if (arguments[1].ToLower() == "health")
                {
                    _instance.Health.Value = arguments[2];
                }
                else if (arguments[1].ToLower() == "damage")
                {
                    _instance.Damage.Value = arguments[2];
                } else {
                    _instance.Health.Value = arguments[1];
                    _instance.Damage.Value = arguments[2];
                    _instance.Config.Save();
                }
                GameUIAwake();
            }

            qol_core.Plugin.SendMessage($"Health: {_instance.Health.Value} Damage: {_instance.Damage.Value}", _modInstance);

            return true;
        }
    }
}