using System;
using System.Reflection;
using HarmonyLib;
using PrehistoricKingdom;
using VLib;

namespace NullFenceCollision
{
    // Reflection and source-list checks used by fence registration.
    internal static class StructureDatabaseHelpers
    {
        public static void SetField(
            Type owner, object target, string name, object value)
        {
            FieldInfo field = AccessTools.Field(owner, name);

            if (field == null)
                throw new MissingFieldException(owner.FullName, name);

            field.SetValue(target, value);
        }

        public static void CheckForIDCollision(BuildingDatabaseV2 database)
        {
            // Does anyone read these?
            foreach (FieldInfo field in typeof(BuildingDatabaseV2).GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic))
            {
                Type type = field.FieldType;

                if (!type.IsGenericType ||
                    type.GetGenericTypeDefinition() != typeof(VSortedList<>))
                {
                    continue;
                }

                var collection = field.GetValue(database)
                    as System.Collections.IEnumerable;

                if (collection == null)
                    continue;

                foreach (object item in collection)
                {
                    var entry = item as IDBEntry_Structure;
                    StructureData data = entry?.StructureData;

                    if (data != null && data.UniqueID == CustomHabitatMarker.FenceID)
                        throw new InvalidOperationException(
                            "Custom fence ID conflicts with: " + data.Name);
                }
            }
        }
    }
}
