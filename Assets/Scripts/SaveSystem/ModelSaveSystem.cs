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
    private static Dictionary<int,SavedModel> _saves;
    private ModelSaveSystem() 
    {
        Init();
    }

    private void FillSavesDictionary()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        modelList.List = new List<SavedModel>();
        string existingSave = File.ReadAllText(_path + "/Save.json");
        modelList = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        if(modelList != null )
        {
            foreach (SavedModel model in modelList.List)
            {
                _saves.Add(model.ID, model);
            }
        }
    }

    public void Init()
    {
        _saves = new Dictionary<int, SavedModel>();
        CheckForFileAndDirectories();
        FillSavesDictionary();
    }

    private void CheckForFileAndDirectories()
    {
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        if (!File.Exists(_path + "/Save.json"))
        {
            File.Create(_path + "/Save.json").Close();
        }
    }
    private void RewriteFile()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        foreach (KeyValuePair<int, SavedModel> model in _saves)
        {
            modelList.List.Add(model.Value);
        }
        string newJsonSave = JsonUtility.ToJson(modelList);
        File.WriteAllText(_path + "/Save.json", newJsonSave);
    }

    public void Save(SavedModel savedModel)
    {
        if (_saves.ContainsKey(savedModel.ID))
        {
            _saves[savedModel.ID] = savedModel;
            RewriteFile();
        }
        else
        {
            _saves.Add(savedModel.ID, savedModel);
            RewriteFile();
        }
    }
    public static ModelSaveSystem GetInstance()
    {
        return instance;
    }
    
    public SavedModel Load(PlayerType playerType, int id = -1)
    {
        SavedModel model = FindModelByID(id);
        if (model == null)
        {
            model = FindModelByType(playerType);
        }
        return model;
    }

    private SavedModel FindModelByType(PlayerType type)
    {
        SavedModelCollection collection = new SavedModelCollection();
        List<SavedModel> list = collection.List;
        foreach (KeyValuePair<int, SavedModel> model in _saves)
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

    private SavedModel FindModelByID(int id)
    {
        if (_saves.ContainsKey(id))
            return _saves[id];
        return null;
    }
}
