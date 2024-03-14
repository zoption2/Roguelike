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
    private static Dictionary<CharacterType,CharacterModel> _saves;
    private ModelSaveSystem() 
    {
        Init();
    }

    private void FillSavesDictionary()
    {
        SavedModelCollection modelList = new SavedModelCollection();
        modelList.List = new List<CharacterModel>();
        string existingSave = File.ReadAllText(_path + _saveName);
        modelList = JsonUtility.FromJson<SavedModelCollection>(existingSave);
        if(modelList != null )
        {
            foreach (CharacterModel model in modelList.List)
            {
                _saves.Add(model.Type, model);
            }
        }
    }

    public List<CharacterType> GetAvailablePlayersTypes()
    {
        List<CharacterType> list = new List<CharacterType>();
        foreach(KeyValuePair<CharacterType, CharacterModel> model in _saves)
        {
            list.Add(model.Value.Type);
        }
        return list;
    }
    private void Init()
    {
        _saves = new Dictionary<CharacterType, CharacterModel>();
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
        foreach (KeyValuePair<CharacterType, CharacterModel> model in _saves)
        {
            modelList.List.Add(model.Value);
        }
        string newJsonSave = JsonUtility.ToJson(modelList);
        File.WriteAllText(_path + _saveName, newJsonSave);
    }

    public void Save(CharacterModel savedModel)
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
    
    public CharacterModel Load(CharacterType playerType)
    {
        CharacterModel model = FindModelByType(playerType);
        return model;
    }

    private CharacterModel FindModelByType(CharacterType type)
    {
        SavedModelCollection collection = new SavedModelCollection();
        List<CharacterModel> list = collection.List;
        foreach (KeyValuePair<CharacterType, CharacterModel> model in _saves)
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
