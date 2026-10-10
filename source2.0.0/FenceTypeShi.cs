using HarmonyLib;
using PrehistoricKingdom;
using PrehistoricKingdom.Fences.V2;
using UnityEngine;

namespace NullFenceCollision
{
    [HarmonyPatch(typeof(FenceV2), nameof(FenceV2.SpawnFenceObject))]
    internal static class FenceTypeShi
    {
        [HarmonyPostfix]
        private static void Postfix(FenceV2 fence, FenceObject __result)
        {
            if (fence == null || __result == null)
                return;

            // Only apply collision to our custom Habitat Marker.
            if (!CustomHabitatMarker.IsCustom(fence.Data))
                return;

            foreach (BoxCollider collider in
                __result.GetComponentsInChildren<BoxCollider>(true))
            {
                collider.gameObject.layer = Game.Layers.ModularLayer;
            }
        }
    }
}
