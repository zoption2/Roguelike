using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.IO;

namespace UI
{
    public interface ICharacterPanel
    {
        public SavedModel IDModel { get;}
    }
    public class CharacterPanel : MonoBehaviour, ICharacterPanel
    {
        [field: SerializeField] public SavedModel IDModel { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; set; }
        private Toggle _toggle;
        private IStatsProvider _statsProvider;
        private SavedCharacterModelHolder _savedCharacterModelHolder;
        private bool _isEnabled;

        [Inject]
        public void Construct(IStatsProvider statsProvider,
            SavedCharacterModelHolder savedCharacterModelHolder)
        {
            _savedCharacterModelHolder = savedCharacterModelHolder;
            _statsProvider = statsProvider;
        }

        void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ChangeBool);
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
            SavedModel savedModel = ScriptableObject.CreateInstance<SavedModel>();
            savedModel.Speed = stats.Speed;
            savedModel.Health = stats.Health;
            savedModel.Damage = stats.Damage;
            savedModel.ID = GenerateID();
            IDModel = savedModel;
            DataTransfer.IdCollection.Add(savedModel.ID);
            //_savedCharacterModelHolder.AddModel(savedModel);
            //File.WriteAllText(Application.dataPath + "/Saves/Save.txt", "tasty testing test");
        }

        private int GenerateID()
        {
            int id = Random.Range(1, 1000);
            return id;
        }
    }
}
