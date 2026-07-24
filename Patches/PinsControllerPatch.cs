using HarmonyLib;
using Shared.Pins;

namespace ModMod.Patches;


[HarmonyPatch(typeof(PinsController))]
public static class PinsControllerPatch {
    [HarmonyPatch(typeof(PinsController), nameof(PinsController.IsPinActive))]
    [HarmonyPostfix]
    public static void IsPinActive(ref bool __result, string pinName) {
        if( pin_overrides.Keys.Contains(pinName))
        {
            __result |= pin_overrides[pinName];
            return;
        }
    }
}
