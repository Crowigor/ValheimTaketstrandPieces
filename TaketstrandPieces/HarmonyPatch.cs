using HarmonyLib;
using UnityEngine;

namespace TaketstrandPieces;

[HarmonyPatch(typeof(Container), nameof(Container.Interact))]
public static class ContainerInteractPatch
{
    static bool Prefix(Container __instance, Humanoid character, bool hold, bool alt)
    {
        var piece = __instance.GetComponent<Piece>();
        if (piece == null || piece.m_name != "$tkd_named_chest")
            return true;

        var sign = __instance.GetComponent<Sign>();
        if (sign == null)
            return true;

        if (!alt)
            return true;

        return !sign.Interact(character, hold, true);
    }
}

[HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
public static class ContainerGetHoverTextPatch
{
    static bool Prefix(Container __instance, ref string __result)
    {
        var piece = __instance.GetComponent<Piece>();
        if (piece == null || piece.m_name != "$tkd_named_chest")
            return true;

        var sign = __instance.GetComponent<Sign>();
        if (sign == null)
            return true;

        __result = Localization.instance.Localize(
            $"$tkd_named_chest\n" +
            $"[<color=yellow><b>$KEY_Use</b></color>] $tkd_named_chest_open\n" +
            $"[<color=yellow><b>Shift + $KEY_Use</b></color>] $tkd_named_chest_edit\n" +
            $"[<color=yellow><b>1-8</b></color>] $tkd_named_chest_set_1\n" +
            $"[<color=yellow>Shift + <b>1-8</b></color>] $tkd_named_chest_set_2\n" +
            $"[<color=yellow>Alt + <b>1-8</b></color>] $tkd_named_chest_set_3\n");

        return false;
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.UseHotbarItem))]
public static class InterceptUseHotbarItemOnNamedChest
{
    static bool Prefix(Player __instance, int index)
    {
        var hoverObject = __instance.GetHoverObject();
        if (hoverObject == null)
            return true;
        
        var piece = hoverObject.GetComponentInParent<Piece>();
        if (piece == null || piece.m_name != "$tkd_named_chest")
            return true;
        
        var sign = piece.GetComponent<Sign>();
        if (sign == null)
            return true;
        
        var item = __instance.GetInventory()?.GetItemAt(index - 1, 0);
        if (item == null)
            return true;

        var prefabName = item.m_dropPrefab ? item.m_dropPrefab.name : item.m_shared.m_name;
        
        var suffix = "#_mode_all";
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            suffix = "#_mode_text";
        else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            suffix = "#_mode_icon";
        
        sign.SetText("#" + prefabName + suffix);
        
        return false;
    }
}