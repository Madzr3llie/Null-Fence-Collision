using System;
using System.Collections.Generic;
using GPUInstancer;
using MelonLoader;
using UnityEngine;

namespace NullFenceCollision
{
    // Copies visual assets and recolors only the copied panel material.
    internal static class MarkerAppearance
    {
        private static GameObject prefabStorage;

        private static readonly Dictionary<Material, Material> copiedMaterials =
            new Dictionary<Material, Material>();

        public static GPUInstancerPrefabPrototype ClonePrototype(
            GPUInstancerPrefabPrototype original, string suffix)
        {
            if (original == null || original.prefabObject == null)
            {
                throw new InvalidOperationException(
                    "Missing vanilla prototype or prefab: " + suffix);
            }

            if (prefabStorage == null)
            {
                prefabStorage = new GameObject("NullFenceCollision_Prefabs");

                prefabStorage.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(prefabStorage);
            }

            GPUInstancerPrefabPrototype clone =
                UnityEngine.Object.Instantiate(original);

            clone.name = "NullCollision_" + suffix;

            GameObject prefabCopy = UnityEngine.Object.Instantiate(
                original.prefabObject, prefabStorage.transform, false);

            prefabCopy.name = "NullCollision_" + suffix + "_Prefab";

            foreach (Renderer renderer in
                prefabCopy.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;

                for (int i = 0; i < materials.Length; i++)
                {
                    Material source = materials[i];

                    if (source == null)
                        continue;

                    // Match the blue panel material; keep white posts unchanged.
                    string materialName =
                        source.name.Replace(" (Instance)", "").Trim();

                    if (materialName != "NullFence")
                        continue;

                    if (!copiedMaterials.TryGetValue(source, out Material redCopy))
                    {
                        // A distinct material lets vanilla retain its blue color.
                        redCopy = new Material(source);
                        redCopy.name = "NullFenceCollision_Red";

                        SetRedColors(redCopy);
                        copiedMaterials.Add(source, redCopy);

                        MelonLogger.Msg(
                            "Created red copy of material: " + source.name);
                    }

                    materials[i] = redCopy;
                }

                renderer.sharedMaterials = materials;
            }

            clone.prefabObject = prefabCopy;
            return clone;
        }

        private static void SetRedColors(Material material)
        {
            if (material.HasProperty("_Tint"))
                material.SetColor("_Tint", new Color(0.85f, 0.38f, 0.38f, 1f));

            if (material.HasProperty("_Tint1"))
                material.SetColor("_Tint1", new Color(0.77f, 0.11f, 0.11f, 1f));

            if (material.HasProperty("_Tint2"))
                material.SetColor("_Tint2", new Color(0.76f, 0.49f, 0.49f, 1f));

            if (material.HasProperty("_IntersectEmission"))
                material.SetColor(
                    "_IntersectEmission", new Color(2f, 0.30f, 0.30f, 0f));
        }
    }
}
