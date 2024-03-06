using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;
using UnityEditor;

namespace UI
{
    public interface ICharacterPanel
    {
        public SavedModel IDModel { get;}
    }
    public class CharacterPanel : MonoBehaviour, ICharacterPanel
    {
        private const string LAST_ID= "LastID";
        private readonly string _path = Application.dataPath + "/Saves/";
        public SavedModel IDModel { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
        private IStatsProvider _statsProvider;
        private bool _isEnabled;
        private SimpleSaveSystem _saveSystem;

        [Inject]
        public void Construct(IStatsProvider statsProvider)
        {
            _statsProvider = statsProvider;
        }

        void Start()
        {
            _saveSystem = SimpleSaveSystem.GetInstance();
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ChangeBool);
            Init();
        }

        public void Init()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            InitModel();
        }
        public void InitModel()
        {
            IDModel = _saveSystem.Load(PlayerType);
        }
        
        public void ChangeBool(bool toggleValue)
        {
            _isEnabled = toggleValue;
            if(_isEnabled && IDModel == null)
            {
                CreateSavedModel();
            }
        }

        private void CreateSavedModel()
        {
            Stats stats = _statsProvider.GetCharacterStats(PlayerType);
            int id = GenerateID();
            SavedModel savedModel = new SavedModel(stats,id,PlayerType);
            IDModel = savedModel;
            //DataTransfer.IdCollection.Add(savedModel.ID);
            _saveSystem.NewSave(savedModel);
        }

        private int GenerateID()
        {
            int lastID = PlayerPrefs.GetInt(LAST_ID, 1);
            PlayerPrefs.SetInt(LAST_ID, ++lastID);
            return lastID;
        }
    }
}
