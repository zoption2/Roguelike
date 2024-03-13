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
    private static Dictionary<PlayerType,PlayerModel> _saves;
    private ModelSaveSystem() 
    {
        Init();
    }

    private void FillSavesDictionary()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        modelList.List = new List<PlayerModel>();
        string existingSave = File.ReadAllText(_path + "/Save.json");
        modelList = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        if(modelList != null )
        {
            foreach (PlayerModel model in modelList.List)
            {
                _saves.Add(model.Type, model);
            }
        }
    }

    public List<PlayerType> GetAvailablePlayersTypes()
    {
        List<PlayerType> list = new List<PlayerType>();
        foreach(KeyValuePair<PlayerType, PlayerModel> model in _saves)
        {
            list.Add(model.Value.Type);
        }
        return list;
    }
    private void Init()
    {
        _saves = new Dictionary<PlayerType, PlayerModel>();
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
        foreach (KeyValuePair<PlayerType, PlayerModel> model in _saves)
        {
            modelList.List.Add(model.Value);
        }
        string newJsonSave = JsonUtility.ToJson(modelList);
        File.WriteAllText(_path + "/Save.json", newJsonSave);
    }

    public void Save(PlayerModel savedModel)
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
    
    public PlayerModel Load(PlayerType playerType, int id = -1)
    {
        //SavedPlayerModel model = FindModelByID(id);
        //if (model == null)
        //{
        //    model = FindModelByType(playerType);
        //}
        PlayerModel model = FindModelByType(playerType);
        return model;
    }

    private PlayerModel FindModelByType(PlayerType type)
    {
        SavedModelCollection collection = new SavedModelCollection();
        List<PlayerModel> list = collection.List;
        foreach (KeyValuePair<PlayerType, PlayerModel> model in _saves)
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

    //private SavedPlayerModel FindModelByID(int id)
    //{
    //    if (_saves.ContainsKey(id))
    //        return _saves[id];
    //    return null;
    //}
}
