using HarmonyLib;
using RhythmRift;
using Shared.Pins;

namespace ModMod.Patches;


[HarmonyPatch(typeof(RRStageController))]
public static class RRStageControllerPatch {
    [HarmonyPatch(typeof(RRStageController), nameof(RRStageController.ApplyActivePinEffects))]
    [HarmonyPostfix]
    public static void ApplyActivePinEffects( RRStageController __instance ) {
        if(PinsController.IsPinActive("Gloom")) {
            for (int i = 0; i < Modifier.GloomRowOverride; i++) {
                var animator = __instance._obscureAnimators[i];
                if (animator) {
                    animator.SetTrigger("GloomOn");
                }
            }
        }
    }
}
