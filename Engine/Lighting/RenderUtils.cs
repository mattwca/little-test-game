using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace Engine.Lighting;

internal static class RenderUtils
{
    public record LightOccluderColour
    {
        public Color Colour { get; set; }
        public string? EntityName { get; set; }
    }

    /// <summary>
    /// A dictionary of assigned light occluder colours.
    /// </summary>
    private static readonly List<LightOccluderColour> _lightOccluderColours = new(256);

    public static LightOccluderColour AssignLightOccluder(string entityName)
    {
        var freeOccluderIndex = _lightOccluderColours.FindIndex((occluder) => occluder.EntityName == null);
        if (freeOccluderIndex == -1)
        {
            throw new System.Exception("No occluder exists");
        }

        var freeOccluder = _lightOccluderColours[freeOccluderIndex];

        var updatedOccluder = freeOccluder with { EntityName = entityName };

        _lightOccluderColours[freeOccluderIndex] = updatedOccluder;
        return updatedOccluder;
    }
}