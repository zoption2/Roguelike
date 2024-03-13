using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ModelSaveSystem
{
    private static ModelSaveSystem instance = new ModelSaveSystem();
    private readonly string _path = Application.dataPath + "/Saves/";
    private readonly string _saveName = "/Save.json";
    private static Dictionary<PlayerType,SavedPlayerModel> _saves;
    private ModelSaveSystem() 
    {
        Init();
    }

    private void FillSavesDictionary()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        modelList.List = new List<SavedPlayerModel>();
        string existingSave = File.ReadAllText(_path + _saveName);
        modelList = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        if(modelList != null )
        {
            foreach (SavedPlayerModel model in modelList.List)
            {
                _saves.Add(model.Type, model);
            }
        }
    }

    public List<PlayerType> GetAvailablePlayersTypes()
    {
        List<PlayerType> list = new List<PlayerType>();
        foreach(KeyValuePair<PlayerType, SavedPlayerModel> model in _saves)
        {
            list.Add(model.Value.Type);
        }
        return list;
    }
    private void Init()
    {
        _saves = new Dictionary<PlayerType, SavedPlayerModel>();
        CheckForFileAndDirectories();
        FillSavesDictionary();
    }

    private void CheckForFileAndDirectories()
    {
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        if (!File.Exists(_path + _saveName))
        {
            File.Create(_path + _saveName).Close();
        }
    }
    private void RewriteFile()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        foreach (KeyValuePair<PlayerType, SavedPlayerModel> model in _saves)
        {
            modelList.List.Add(model.Value);
        }
        string newJsonSave = JsonUtility.ToJson(modelList);
        File.WriteAllText(_path + _saveName, newJsonSave);
    }

    public void Save(SavedPlayerModel savedModel)
    {
        if (_saves.ContainsKey(savedModel.Type))
        {
            _saves[savedModel.Type] = savedModel;
            RewriteFile();
        }
        else
        {
            _saves.Add(savedModel.Type, savedModel);
            RewriteFile();
        }
    }
    public static ModelSaveSystem GetInstance()
    {
        return instance;
    }
    
    public SavedPlayerModel Load(PlayerType playerType)
    {
        SavedPlayerModel model = FindModelByType(playerType);
        return model;
    }

    private SavedPlayerModel FindModelByType(PlayerType type)
    {
        SavedModelCollection collection = new SavedModelCollection();
        List<SavedPlayerModel> list = collection.List;
        foreach (KeyValuePair<PlayerType, SavedPlayerModel> model in _saves)
        {
            list.Add(model.Value);
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Type.Equals(type))
                return list[i];
        }
        return null;
    }
}
