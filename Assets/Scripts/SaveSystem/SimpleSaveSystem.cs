using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SimpleSaveSystem
{
    private static SimpleSaveSystem instance = new SimpleSaveSystem();

    private readonly string _path = Application.dataPath + "/Saves/";
    private SimpleSaveSystem() 
    {
        Init();
    }

    public void Init()
    {
        if (!File.Exists(_path + "/Save.json"))
        {
            File.Create(_path + "/Save.json");
            Debug.Log("Save file was created");
        }
    }

    public void NewSave(SavedModel savedModel)
    {
        string existingSave = File.ReadAllText(_path + "/Save.json");
        SavedModelCollection list = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        if (list == null)
            list = new SavedModelCollection();
        list.List.Add(savedModel);
        Debug.Log(JsonUtility.ToJson(savedModel));
        string newJsonSave = JsonUtility.ToJson(list);
        Debug.Log(newJsonSave);
        File.WriteAllText(_path + "/Save.json", newJsonSave);
        Debug.Log("Save was created");
    }
    public static SimpleSaveSystem GetInstance()
    {
        return instance;
    }
    
    public SavedModel Load(PlayerType playerType)
    {
        string existingSave = File.ReadAllText(_path + "/Save.json");
        SavedModelCollection list = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        Debug.Log("Model was loaded");
        if (list == null)
            list = new SavedModelCollection();
        SavedModel model = FindNeededModel(list.List, playerType);
        return model;
    }

    public SavedModel FindNeededModel(List<SavedModel> list, PlayerType type)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Type.Equals(type))
                return list[i];
        }
        return null;
    }
}
