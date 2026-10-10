using HarmonyLib;
using PrehistoricKingdom;
using PrehistoricKingdom.Fences.V2;

namespace NullFenceCollision
{
    [HarmonyPatch(
        typeof(FenceManagerV2),
        "SetNullFenceVisibility",
        new[] { typeof(bool) })]
    internal static class CustomMarkerVisibilityPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            FenceManagerV2 __instance, bool visible)
        {
            if (!PKPersistentData.BuildDB.TryGetFence(
                CustomHabitatMarker.FenceName, out FenceData data))
            {
                return;
            }

            if (!CustomHabitatMarker.IsCustom(data))
                return;

            var renderer = __instance.Renderer;

            if (renderer == null)
                return;

            renderer.FetchRenderStack(
                data, data.SubSectionPrototype, true).Draw = visible;

            renderer.FetchRenderStack(
                data, data.MainPostPrototype, true).Draw = visible;

            renderer.FetchRenderStack(
                data, data.SubPostPrototype, true).Draw = visible;

            __instance.SetRenderDirty();
        }
    }
}