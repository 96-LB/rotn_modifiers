using HarmonyLib;
using Shared.SceneLoading.Payloads;

namespace ModMod.Patches;


[HarmonyPatch(typeof(RRDynamicScenePayload))]
public static class RRDynamicScenePayloadPatch {
    [HarmonyPatch(typeof(RRDynamicScenePayload), nameof(RRDynamicScenePayload.FromMetadata))]
    [HarmonyPostfix]
    public static void FromMetadata(RRDynamicScenePayload __result) {
        __result.ShouldProcGen = Modifier.IsEnabled("Remix");
    }
}
