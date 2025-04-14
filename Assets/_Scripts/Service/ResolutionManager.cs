using System.Collections.Generic;
using UnityEngine;

public static class ResolutionManager
{
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

    public static List<string> GetResolutionOptions()
    {
        var list = new List<string>();
        foreach (var res in GetUniqueResolutions())
        {
            list.Add($"{res.width}x{res.height}");
        }
        return list;
    }

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

    public static Vector2Int GetResolutionByIndex(int index)
    {
        var res = GetUniqueResolutions();
        return new Vector2Int(res[index].width, res[index].height);
    }
}
