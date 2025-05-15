using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A serializable dictionary for use with Unity's inspector and serialization system.
/// Converts dictionary contents to parallel key and value lists for serialization.
/// </summary>
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    /// <summary>
    /// Serialized list of keys corresponding to the dictionary entries.
    /// </summary>
    [SerializeField] private List<TKey> keys = new List<TKey>();

    /// <summary>
    /// Serialized list of values corresponding to the dictionary entries.
    /// </summary>
    [SerializeField] private List<TValue> values = new List<TValue>();

    /// <summary>
    /// Called before Unity serializes the object.
    /// Populates the key and value lists from the dictionary.
    /// </summary>
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    /// <summary>
    /// Called after Unity deserializes the object.
    /// Reconstructs the dictionary from the key and value lists.
    /// Logs an error if the data is mismatched or corrupted.
    /// </summary>
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("SerializableDictionary: Key and value count mismatch during deserialization.");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
