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
            if (fence == null || fence.Data == null || !fence.Data.IsMarker || __result == null)
                return;
            if (!PKPersistentData.BuildDB.TryGetFence("Habitat Marker", out FenceData nullFenceData))
                return;
            if (fence.Data != nullFenceData)
                return;
            foreach (BoxCollider collider in __result.GetComponentsInChildren<BoxCollider>(true))
            { collider.gameObject.layer = Game.Layers.ModularLayer;
            }
        }
    }
}
