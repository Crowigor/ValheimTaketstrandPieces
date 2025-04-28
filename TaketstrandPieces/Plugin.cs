using System;
using BepInEx;
using BepInEx.Configuration;
using Jotunn.Managers;
using ServerSync;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Utils;
using UnityEngine;
using TMPro;

namespace TaketstrandPieces;

[BepInPlugin(ModGuid, ModName, ModVersion)]
[BepInDependency("com.jotunn.jotunn")]
public class TaketstrandPieces : BaseUnityPlugin
{
    private const string ModGuid = "com.crowigor.taketstrandpieces";
    private const string ModName = "TaketstrandPieces";
    private const string ModVersion = "1.0.0";
    const string AssetBundleName = "taketstrand";

    private AssetBundle _bundle;
    private ConfigEntry<bool> _debug;
    private Harmony _harmony;

    public EffectList effectBreakWood;
    public EffectList effectHitWood;
    public EffectList effectBuildWood;
    public EffectList effectBreakGlass;
    public EffectList effectBuildGlass;
    public EffectList effectDoorOpen;
    public EffectList effectDoorClose;
    public EffectList effectGateOpen;
    public EffectList effectGateClose;

    private readonly Dictionary<string, CustomPieceConfig> _pieceConfigs = new()
    {
        ["tkd_piece_ashwood_door"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "piece_workbench",
            [
                new RequirementConfig("Blackwood", 12)
            ]
        ),
        ["tkd_piece_ashwood_door_1"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "piece_artisanstation",
            [
                new RequirementConfig("Blackwood", 7),
                new RequirementConfig("Crystal", 2)
            ]),
        ["tkd_piece_ashwood_door_2"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "piece_artisanstation",
            [
                new RequirementConfig("Blackwood", 5),
                new RequirementConfig("Crystal", 4)
            ]),
        ["tkd_piece_ashwood_door_3"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "piece_artisanstation",
            [
                new RequirementConfig("Blackwood", 4),
                new RequirementConfig("Crystal", 4)
            ]),
        ["tkd_piece_ashwood_door_4"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "piece_artisanstation",
            [
                new RequirementConfig("Blackwood", 6),
                new RequirementConfig("Crystal", 4)
            ]),
        ["tkd_piece_ashwood_gate"] = new CustomPieceConfig(
            "BuildingWorkbench",
            "blackforge",
            [
                new RequirementConfig("Blackwood", 16),
                new RequirementConfig("FlametalNew", 4)
            ]),
        ["tkd_piece_glass_wall"] = new CustomPieceConfig(
            "BuildingStonecutter",
            "piece_artisanstation",
            [
                new RequirementConfig("Crystal", 6)
            ]),
        ["tkd_piece_glass_wall_half"] = new CustomPieceConfig(
            "BuildingStonecutter",
            "piece_artisanstation",
            [
                new RequirementConfig("Crystal", 3)
            ]),
        ["tkd_piece_named_chest"] = new CustomPieceConfig(
            "Furniture",
            "piece_workbench",
            [
                new RequirementConfig("FineWood", 10),
                new RequirementConfig("Iron", 2),
                new RequirementConfig("Coal", 1)
            ]
        ),
    };

    private class CustomPieceConfig(
        string category = "Misc",
        string craftingStation = null,
        RequirementConfig[] cost = null)
    {
        public readonly string Category = category;
        public readonly string CraftingStation = craftingStation;
        public readonly RequirementConfig[] Requirements = cost;
    }

    private static readonly ConfigSync ConfigSync = new(ModGuid)
    {
        DisplayName = ModName,
        CurrentVersion = ModVersion,
        MinimumRequiredVersion = ModVersion
    };

    private void Awake()
    {
        const string section = "General";
        _debug = ConfigEntry(section, "Debug", false, "Enable debug logging");

        LoadEmbeddedLocalizations();

        _bundle = AssetUtils.LoadAssetBundleFromResources(AssetBundleName, typeof(TaketstrandPieces).Assembly);
        if (_bundle == null)
        {
            LogError($"❌ Failed to load AssetBundle: {AssetBundleName}");
            return;
        }

        LogInfo($"✅ Load AssetBundle: {AssetBundleName}");

        _harmony = new Harmony(ModGuid);
        _harmony.PatchAll();

        PrefabManager.OnVanillaPrefabsAvailable += LoadPieces;
    }

    private void LoadEmbeddedLocalizations()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();

        var localization = LocalizationManager.Instance.GetLocalization();

        foreach (var resourceName in resourceNames)
        {
            if (!resourceName.Contains("Localization.") || !resourceName.EndsWith(".json"))
                continue;

            var nameParts = resourceName.Split('.');
            var language = nameParts[nameParts.Length - 2];

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                localization.AddJsonFile(language, json);
            }

            LogInfo($"🌐 Loaded localization: {language} from {resourceName}");
        }
    }

    private void LoadPieces()
    {
        RegisterEffects();
        foreach (var assetPath in _bundle.GetAllAssetNames())
        {
            var prefabName = Path.GetFileNameWithoutExtension(assetPath);
            if (prefabName.Contains("tkd_piece"))
            {
                LoadPiece(prefabName);
            }
        }

        CreateNamedChest();
    }

    private void RegisterEffects()
    {
        var sfxWoodBuild = PrefabManager.Cache.GetPrefab<GameObject>("sfx_build_hammer_wood");
        var vfxWoodBuild = PrefabManager.Cache.GetPrefab<GameObject>("vfx_Place_wood_wall");
        effectBuildWood = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxWoodBuild },
                new EffectList.EffectData { m_prefab = vfxWoodBuild }
            ]
        };

        var sfxWoodBreak = PrefabManager.Cache.GetPrefab<GameObject>("sfx_wood_break");
        var vfxWoodHit = PrefabManager.Cache.GetPrefab<GameObject>("vfx_SawDust");
        effectBreakWood = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxWoodBreak },
                new EffectList.EffectData { m_prefab = vfxWoodHit }
            ]
        };
        effectHitWood = new EffectList
        {
            m_effectPrefabs = [new EffectList.EffectData { m_prefab = vfxWoodHit }]
        };

        var sfxBuildHammerCrystal = PrefabManager.Cache.GetPrefab<GameObject>("sfx_build_hammer_crystal");
        var vfxPlaceWoodPole = PrefabManager.Cache.GetPrefab<GameObject>("vfx_Place_wood_pole");
        effectBuildGlass = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxBuildHammerCrystal },
                new EffectList.EffectData { m_prefab = vfxPlaceWoodPole }
            ]
        };

        var fxCrystalDestruction = PrefabManager.Cache.GetPrefab<GameObject>("fx_crystal_destruction");
        effectBreakGlass = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = fxCrystalDestruction }
            ]
        };


        var sfxDoorOpen = PrefabManager.Cache.GetPrefab<GameObject>("sfx_door_open");
        effectDoorOpen = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxDoorOpen }
            ]
        };

        var sfxDoorClose = PrefabManager.Cache.GetPrefab<GameObject>("sfx_door_close");
        effectDoorClose = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxDoorClose }
            ]
        };

        var sfxGateOpen = PrefabManager.Cache.GetPrefab<GameObject>("sfx_darkwood_door_open");
        effectGateOpen = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxGateOpen }
            ]
        };

        var sfxGateClose = PrefabManager.Cache.GetPrefab<GameObject>("sfx_darkwood_door_close");
        effectGateClose = new EffectList
        {
            m_effectPrefabs =
            [
                new EffectList.EffectData { m_prefab = sfxGateClose }
            ]
        };
    }

    private void LoadPiece(string prefabName)
    {
        var prefab = _bundle.LoadAsset<GameObject>(prefabName);
        if (prefab == null)
        {
            LogWarning($"❌ Prefab not found in bundle: {prefabName}");
            return;
        }

        if (prefabName.Contains("wood"))
        {
            var fxBuild = prefab.GetComponent<Piece>();
            fxBuild.m_placeEffect = effectBuildWood;

            var fxHit = prefab.GetComponent<WearNTear>();
            fxHit.m_hitEffect = effectHitWood;
            fxHit.m_destroyedEffect = effectBreakWood;
        }
        else if (prefabName.Contains("glass"))
        {
            var fxBuild = prefab.GetComponent<Piece>();
            fxBuild.m_placeEffect = effectBuildGlass;

            var fxHit = prefab.GetComponent<WearNTear>();
            fxHit.m_destroyedEffect = effectBreakGlass;
        }

        if (prefabName.Contains("door"))
        {
            var door = prefab.GetComponent<Door>();
            door.m_openEffects = effectDoorOpen;
            door.m_closeEffects = effectDoorClose;
        }
        else if (prefabName.Contains("gate"))
        {
            var door = prefab.GetComponent<Door>();
            door.m_openEffects = effectGateOpen;
            door.m_closeEffects = effectGateClose;
        }

        var piece = new CustomPiece(prefab, true, GetPieceConfig(prefabName));

        PieceManager.Instance.AddPiece(piece);
        LogInfo($"✅ Registered piece: {prefabName}");
    }

    private void CreateNamedChest()
    {
        if (PrefabManager.Instance.GetPrefab("piece_chest") == null)
            return;

        var signPrefab = PrefabManager.Instance.GetPrefab("sign");
        if (signPrefab == null)
            return;

        var chest = PrefabManager.Instance.CreateClonedPrefab("tkd_piece_named_chest", "piece_chest");
        var piece = chest.GetComponent<Piece>();
        piece.m_name = "$tkd_named_chest";

        var signCanvas = Helper.CreateSignCanvas("SignCanvas", chest.transform,
            new Vector3(), Quaternion.Euler(0, 0, 0), new Vector2(1f, 1f));
        var textWidget = Helper.CreateText(signCanvas.transform, "Text");
        textWidget.fontSize = 0;
        textWidget.color = new Color(0, 0, 0, 0);

        var existingText = signPrefab.GetComponentInChildren<TextMeshProUGUI>(true);
        if (existingText != null)
        {
            textWidget.font = existingText.font;
            textWidget.material = existingText.fontMaterial;
        }

        var sign = chest.AddComponent<Sign>();
        sign.m_textWidget = textWidget;
        sign.m_defaultText = "...";
        sign.m_characterLimit = 80;

        var smart = chest.AddComponent<NamedChest>();
        smart.sign = sign;

        var canvasFront = Helper.CreateSignCanvas("CanvasFront", chest.transform,
            new Vector3(0f, 0.45f, 0.465f), Quaternion.identity, new Vector2(1.4f, 0.4f));
        smart.textFront = Helper.CreateText(canvasFront.transform, "TextFront");
        smart.textFront.rectTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
        smart.textFront.fontSize = 0.15f;

        var canvasLeft = Helper.CreateSignCanvas("canvasLeft", chest.transform,
            new Vector3(0.85f, 0.5f, 0f), Quaternion.Euler(11.5f, -90f, 0), new Vector2(0.5f, 0.5f));
        smart.iconLeft = Helper.CreateIcon(canvasLeft.transform, "IconRight");

        var canvasRight = Helper.CreateSignCanvas("CanvasRight", chest.transform,
            new Vector3(-0.85f, 0.5f, 0f), Quaternion.Euler(11.5f, 90f, 0), new Vector2(0.5f, 0.5f));
        smart.iconRight = Helper.CreateIcon(canvasRight.transform, "IconRight");

        Helper.CreateSnapPoint(chest.transform, new Vector3(-1f, 0f, -0.5f), "$tkd_snappoint_1");
        Helper.CreateSnapPoint(chest.transform, new Vector3(1f, 0f, -0.5f), "$tkd_snappoint_2");
        Helper.CreateSnapPoint(chest.transform, new Vector3(-1f, 0f, 0.5f), "$tkd_snappoint_3");
        Helper.CreateSnapPoint(chest.transform, new Vector3(1f, 0f, 0.5f), "$tkd_snappoint_4");

        PieceManager.Instance.AddPiece(new CustomPiece(chest, true,
            GetPieceConfig("tkd_piece_named_chest")));

        LogInfo($"✅ Registered piece: tkd_piece_named_chest");
    }


    private PieceConfig GetPieceConfig(string prefabName)
    {
        if (!_pieceConfigs.TryGetValue(prefabName, out var config))
        {
            LogWarning($"⚠ Config not found: {prefabName}");
            config = new CustomPieceConfig();
        }

        var pieceConfig = new PieceConfig
        {
            Enabled = true,
            PieceTable = "Hammer",
            Category = config.Category
        };
        pieceConfig.Enabled = true;
        pieceConfig.PieceTable = "Hammer";
        if (!string.IsNullOrEmpty(config.CraftingStation))
        {
            pieceConfig.CraftingStation = config.CraftingStation;
        }

        if (config.Requirements != null)
        {
            pieceConfig.Requirements = config.Requirements;
        }

        return pieceConfig;
    }

    private ConfigEntry<T> ConfigEntry<T>(string section, string key, T value, string description, int order = 0,
        bool synced = true)
    {
        var attributes = new ConfigurationManagerAttributes { Order = order };
        var entry = Config.Bind(section, key, value, new ConfigDescription(description, null, attributes));
        if (!synced)
            return entry;

        ConfigSync.AddConfigEntry(entry);

        entry.SettingChanged += (_, _) =>
        {
            LogInfo($"🛠️ Changed setting → <[{section}] - {key}");

            if (section.Equals("General", StringComparison.OrdinalIgnoreCase))
            {
                LogInfo("⚙️ General setting changed");
            }
        };

        return entry;
    }

    private void LogInfo(string message)
    {
        if (_debug.Value)
            Logger.LogInfo(message);
    }

    private void LogWarning(string message)
    {
        if (_debug.Value)
            Logger.LogWarning(message);
    }

    private void LogError(string message)
    {
        if (_debug.Value)
            Logger.LogError(message);
    }
}