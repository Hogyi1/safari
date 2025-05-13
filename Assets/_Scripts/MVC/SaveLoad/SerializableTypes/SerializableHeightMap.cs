using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableHeightMap
{
    public int width;
    public int height;
    public List<float> data;

    public SerializableHeightMap() { }

    public SerializableHeightMap(float[,] heights)
    {
        height = heights.GetLength(0);
        width = heights.GetLength(1);
        data = new List<float>(height * width);
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                data.Add(heights[i, j]);
    }

    public float[,] ToArray()
    {
        float[,] result = new float[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                result[i, j] = data[i * width + j];
        return result;
    }
}

