using System;
using System.IO;
using UnityEngine;

namespace NullFenceCollision
{
    internal static class MarkerIcon
    {
        private static Sprite cachedIcon;

        public static Sprite GetIcon()
        {
            // Load once, then reuse the sprite.
            if (cachedIcon != null)
                return cachedIcon;

            var assembly = typeof(MarkerIcon).Assembly;

            // Find the PNG without depending on the project's root namespace.
            string resourceName = null;

            foreach (string name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(
                    ".HabitatMarkerCollisionIcon.png",
                    StringComparison.Ordinal))
                {
                    resourceName = name;
                    break;
                }
            }

            if (resourceName == null)
            {
                throw new InvalidOperationException(
                    "The collision fence icon is missing from the DLL. " +
                    "Check that its Build Action is Embedded Resource.");
            }

            byte[] imageBytes;

            using (Stream resource =
                assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                    throw new InvalidOperationException(
                        "Could not open the embedded fence icon.");

                using (var buffer = new MemoryStream())
                {
                    resource.CopyTo(buffer);
                    imageBytes = buffer.ToArray();
                }
            }

            var texture = new Texture2D(
                2, 2, TextureFormat.RGBA32, false);

            texture.name = "HabitatMarkerCollisionIcon";
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            // LoadImage sets the texture dimensions from the PNG.
            if (!ImageConversion.LoadImage(texture, imageBytes, true))
            {
                UnityEngine.Object.Destroy(texture);
                throw new InvalidOperationException(
                    "Could not decode the collision fence icon PNG.");
            }

            cachedIcon = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);

            cachedIcon.name = "HabitatMarkerCollisionIcon";

            return cachedIcon;
        }
    }
}