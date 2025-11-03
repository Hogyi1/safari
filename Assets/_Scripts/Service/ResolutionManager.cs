using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides utilities for retrieving and selecting unique screen resolutions.
/// </summary>
public static class ResolutionManager
{
    /// <summary>
    /// Returns a list of unique screen resolutions supported by the device.
    /// </summary>
    /// <returns>List of unique Resolution structs.</returns>
    public static List<Resolution> GetUniqueResolutions()
    {
        var all = Screen.resolutions;
        var unique = new HashSet<string>();
        var results = new List<Resolution>();

        foreach (var res in all)
        {
            string id = $"{res.width}x{res.height}";
            if (unique.Add(id))
            {
                results.Add(res);
            }
        }

        return results;
    }

    /// <summary>
    /// Retrieves string options for each unique resolution (e.g., "1920x1080").
    /// </summary>
    /// <returns>List of resolution option strings.</returns>
    public static List<string> GetResolutionOptions()
    {
        var list = new List<string>();
        foreach (var res in GetUniqueResolutions())
        {
            list.Add($"{res.width}x{res.height}");
        }
        return list;
    }

    /// <summary>
    /// Finds the index of a target resolution in the unique resolutions list.
    /// </summary>
    /// <param name="target">The resolution to locate.</param>
    /// <returns>Index of the resolution, or 0 if not found.</returns>
    public static int FindResolutionIndex(Vector2Int target)
    {
        var resolutions = GetUniqueResolutions();
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == target.x && resolutions[i].height == target.y)
                return i;
        }
        return 0;
    }

    /// <summary>
    /// Returns the Vector2Int representation of the resolution at the given index.
    /// </summary>
    /// <param name="index">Index into the unique resolutions list.</param>
    /// <returns>Vector2Int with width and height.</returns>
    public static Vector2Int GetResolutionByIndex(int index)
    {
        var res = GetUniqueResolutions();
        return new Vector2Int(res[index].width, res[index].height);
    }
}
