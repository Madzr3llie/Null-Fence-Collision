using System;
using HarmonyLib;
using MelonLoader;
using PrehistoricKingdom;

namespace NullFenceCollision
{
    [HarmonyPatch(typeof(BuildingDatabaseV2),
        nameof(BuildingDatabaseV2.InitRuntimeCacheIfNeedbe))]
    internal static class RegisterCustomMarkerPatch
    {
        [HarmonyPrefix]
        private static void Prefix(
            BuildingDatabaseV2 __instance, out bool __state)
        {
            __state = false;

            try
            {
                __state = CustomHabitatMarker.Register(__instance);
            }
            catch (Exception exception)
            {
                MelonLogger.Error(
                    "Custom fence registration failed:\n" + exception);
            }
        }

        [HarmonyPostfix]
        private static void Postfix(
            BuildingDatabaseV2 __instance, bool __state)
        {
            if (!__state)
                return;

            if (__instance.TryGetFence(
                CustomHabitatMarker.FenceName, out FenceData data) &&
                CustomHabitatMarker.IsCustom(data))
            {
                MelonLogger.Msg(
                    "Custom Habitat Marker is available through the database.");
            }
            else
            {
                MelonLogger.Error(
                    "Custom Habitat Marker is missing from the database cache.");
            }
        }
    }
}
