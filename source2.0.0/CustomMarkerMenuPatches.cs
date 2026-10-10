using HarmonyLib;
using PrehistoricKingdom;

namespace NullFenceCollision
{
    [HarmonyPatch(typeof(StructureData), "get_LocalizedName")]
    internal static class CustomMarkerNamePatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            StructureData __instance, ref string __result)
        {
            if (CustomHabitatMarker.IsCustom(__instance))
                __result = CustomHabitatMarker.FenceName;
        }
    }

    [HarmonyPatch(typeof(FenceData), nameof(FenceData.IsUnlockedInGUI))]
    internal static class CustomMarkerUnlockPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            FenceData __instance, ref bool __result)
        {
            if (CustomHabitatMarker.IsCustom(__instance) &&
                CustomHabitatMarker.VanillaData != null)
            {
                __result =
                    CustomHabitatMarker.VanillaData.IsUnlockedInGUI();
            }
        }
    }
}