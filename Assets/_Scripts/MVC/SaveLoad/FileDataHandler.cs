using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


/// <summary>
/// Handles saving and loading of game data to and from the file system.
/// Supports multiple profiles and optional encryption.
/// </summary>
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "Xe9vL2rMqT1fBg7D";

    /// <summary>
    /// Creates a new FileDataHandler instance for handling file I/O operations.
    /// </summary>
    /// <param name="dataDirPath">The base directory path where data will be stored.</param>
    /// <param name="dataFileName">The name of the file to read/write.</param>
    /// <param name="useEncryption">Whether to encrypt/decrypt the data during save/load.</param>
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    /// <summary>
    /// Loads the game data for a given profile ID from file.
    /// </summary>
    /// <param name="profileId">The profile ID to load data for.</param>
    /// <returns>The loaded GameData object, or null if loading failed or no file exists.</returns>
    public GameData Load(string profileId)
    {
        if (profileId == null)
        {
            return null;
        }
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    /// <summary>
    /// Saves the given game data to file under the specified profile ID.
    /// </summary>
    /// <param name="data">The GameData object to save.</param>
    /// <param name="profileId">The profile ID to save the data for.</param>
    public void Save(GameData data, string profileId)
    {
        if (profileId == null)
        {
            return;
        }
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }


    /// <summary>
    /// Loads all game data files for every available profile in the data directory.
    /// </summary>
    /// <returns>A dictionary mapping profile IDs to their corresponding GameData.</returns>
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }
            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }
        return profileDictionary;
    }

    /// <summary>
    /// Determines which profile was most recently updated based on the timestamp in GameData.
    /// </summary>
    /// <returns>The profile ID of the most recently updated profile, or null if none found.</returns>
    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;
            if (gameData == null)
            {
                continue;
            }
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }

    public void Delete(string profileId)
    {
        // base case - if the profileId is null, return right away
        if (profileId == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            // ensure the data file exists at this path before deleting the directory
            if (File.Exists(fullPath))
            {
                // delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete profile data for profileId: "
                + profileId + " at path: " + fullPath + "\n" + e);
        }
    }


    /// <summary>
    /// Encrypts or decrypts the given string using a simple XOR cipher with the encryption key.
    /// </summary>
    /// <param name="data">The input string to encrypt or decrypt.</param>
    /// <returns>The transformed string after XOR operation.</returns>
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}