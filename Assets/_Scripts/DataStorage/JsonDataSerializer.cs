using System.IO;
using UnityEngine;

public class JsonDataSerializer<T>
{
    private string folderPath;
    private string fileName;

    public JsonDataSerializer(string folderPath, string fileName)
    {
        this.folderPath = folderPath;
        this.fileName = fileName;
    }

    public void SaveData(T data)
    {
        string fullFolderPath = Path.Combine(Application.persistentDataPath, folderPath);
        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath);
        }

        string json = JsonUtility.ToJson(data, true);
        string fullPath = Path.Combine(fullFolderPath, fileName);
        Debug.Log("Saving data to: " + fullPath);
        File.WriteAllText(fullPath, json);
    }

    public T LoadData()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, folderPath, fileName);
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning("File not found: " + fullPath);
            return default;
        }

        Debug.Log("Loading data from: " + fullPath);
        string json = File.ReadAllText(fullPath);
        return JsonUtility.FromJson<T>(json);
    }
}
