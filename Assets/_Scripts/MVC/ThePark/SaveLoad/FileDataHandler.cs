using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Profiling;


public class FileDataHandler 
{

   
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";


    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
     
    }

    public GameData Load(string profileId) {
        string fullPath = Path.Combine(dataDirPath,profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath)) {

            try
            {
                string dataToLoad = "";
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
                {

                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        dataToLoad = streamReader.ReadToEnd();

                    }

                }
                if (useEncryption) { 
                
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception)
            {

                Debug.Log("nem sikerult a load filehiba");
            }


        
        }
        return loadedData;
    }

    public void Save(GameData data, string profileId) { 
    string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data,true);


            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create)) {

                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(dataToStore);

                }

            
            }
        }
        catch (Exception)
        {
            Debug.Log("nem sikerult a save filehiba");
        }
    }

    private string EncryptDecrypt(string data) {

        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] | encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }


    public Dictionary<string, GameData> LoadAllProfiles() {
        Dictionary<string, GameData> profileDirectory = new Dictionary<string, GameData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipped directory ");
                continue;
            }

            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileDirectory.Add(profileId, profileData);
            }
            else { Debug.Log("cant load Profile " + profileId); }

        }

       

        return profileDirectory;
    }

}
