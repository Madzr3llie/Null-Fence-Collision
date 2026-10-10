using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using PrehistoricKingdom;
using UnityEngine;
using VLib;

namespace NullFenceCollision
{
    internal static class CustomHabitatMarker
    {
        public const string FenceName = "Habitat Marker (Collision)";

        // Keep this ID unchanged between mod versions.
        public const long FenceID = 0x4E46434D00000001L;

        public static FenceData VanillaData;

        private static readonly MethodInfo CloneMethod =
            AccessTools.Method(typeof(object), "MemberwiseClone");

        public static bool IsCustom(StructureData data)
        {
            return data != null &&
                   data.UniqueID == FenceID &&
                   data.Name == FenceName;
        }

        public static bool Register(BuildingDatabaseV2 database)
        {
            if (database.runtimeCache != null &&
                database.runtimeCache.IsValid())
            {
                return false;
            }

            var fences = (VSortedList<DBEntry_Fence>)
                AccessTools.Field(typeof(BuildingDatabaseV2), "fences")
                    .GetValue(database);

            if (fences == null)
                throw new InvalidOperationException("Fence list is missing.");

            FenceData original = null;
            FenceData existing = null;

            foreach (DBEntry_Fence entry in fences)
            {
                if (entry == null || !entry.HasEntry)
                    continue;

                FenceData data = entry.Entry;

                if (data.Name == "Habitat Marker")
                    original = data;

                if (IsCustom(data))
                    existing = data;
            }

            if (original == null)
                throw new InvalidOperationException(
                    "Could not find the vanilla Habitat Marker.");

            VanillaData = original;

            if (existing != null)
                return false;

            StructureDatabaseHelpers.CheckForIDCollision(database);

            FenceData clone = (FenceData)CloneMethod.Invoke(original, null);

            StructureDatabaseHelpers.SetField(typeof(StructureData), clone, "name", FenceName);
            StructureDatabaseHelpers.SetField(typeof(StructureData), clone, "nameLower",
                FenceName.ToLowerInvariant());
            StructureDatabaseHelpers.SetField(typeof(StructureData), clone, "internalID", FenceID);

            clone.ThemeIndices = new List<int>(original.ThemeIndices);
            clone.ShowInGUI = true;
            clone.BuildingIcon = MarkerIcon.GetIcon();

            // Separate prototypes prevent the two fence types from
            // sharing the same GPU instance buffers.
            StructureDatabaseHelpers.SetField(typeof(FenceData), clone, "subSectionPrototype",
                MarkerAppearance.ClonePrototype(original.SubSectionPrototype, "Section"));

            StructureDatabaseHelpers.SetField(typeof(FenceData), clone, "mainPostPrototype",
                MarkerAppearance.ClonePrototype(original.MainPostPrototype, "MainPost"));

            StructureDatabaseHelpers.SetField(typeof(FenceData), clone, "subPostPrototype",
                MarkerAppearance.ClonePrototype(original.SubPostPrototype, "SubPost"));

            StructureDatabaseHelpers.SetField(typeof(FenceData), clone,
                "<SubSectionPrefab>k__BackingField",
                clone.SubSectionPrototype.prefabObject);

            StructureDatabaseHelpers.SetField(typeof(FenceData), clone,
                "<MainPostPrefab>k__BackingField",
                clone.MainPostPrototype.prefabObject);

            StructureDatabaseHelpers.SetField(typeof(FenceData), clone,
                "<SubPostPrefab>k__BackingField",
                clone.SubPostPrototype.prefabObject);

            DBEntry_Fence newEntry =
                ScriptableObject.CreateInstance<DBEntry_Fence>();

            newEntry.name = "BuildDB_FEN_NullCollision";
            newEntry.Entry = clone;

            newEntry.OnValidate();

            if (!fences.TryAddExclusiveStrict(newEntry))
            {
                UnityEngine.Object.Destroy(newEntry);
                throw new InvalidOperationException(
                    "The new fence could not be inserted.");
            }

            MelonLogger.Msg("Added custom Habitat Marker to the fence list.");
            return true;
        }

    }
}
